using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Peach.Config;
using Peach.Protocol;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNetty.Handlers.Tls;
using Peach.EventArgs;

namespace Peach.Tcp
{
    public class TcpClient<TMessage> : ISocketClient<TMessage> where TMessage : Messaging.IMessage
    {

        private readonly Bootstrap _bootstrap = new Bootstrap();
        private readonly MultithreadEventLoopGroup _group = new MultithreadEventLoopGroup();
      
        private readonly TcpClientOption _clientOption;

        private readonly ConcurrentDictionary<EndPoint, SocketContext<TMessage>> channels
            = new ConcurrentDictionary<EndPoint, SocketContext<TMessage>>();
        private readonly IChannelHandlerPipeline _handlerPipeline;

        public TcpClient(IOptions<TcpClientOption> clientOption, IChannelHandlerPipeline handlerPipeline)
            : this(clientOption.Value, handlerPipeline)
        {

        }
        public TcpClient(IChannelHandlerPipeline handlerPipeline)
            : this(new TcpClientOption(),handlerPipeline)
        {

        }

        public TcpClient(TcpClientOption clientOption, IChannelHandlerPipeline handlerPipeline)
        {
            _clientOption = clientOption;
            _handlerPipeline = handlerPipeline;
            InitBootstrap();
            
        }


        #region Init

        /// <summary>
        /// Init Bootstrap
        /// </summary>
        private void InitBootstrap()
        {
            _bootstrap.Group(_group)
                .Channel<TcpSocketChannel>();

            if (_clientOption.TcpNodelay)
            {
                _bootstrap.Option(ChannelOption.TcpNodelay, true);
            }
            if (_clientOption.SoKeepalive)
            {
                _bootstrap.Option(ChannelOption.SoKeepalive, true);
            }
            if (_clientOption.ConnectTimeout > 0)
            {
                _bootstrap.Option(ChannelOption.ConnectTimeout, TimeSpan.FromMilliseconds(_clientOption.ConnectTimeout));
            }

            X509Certificate2 cert = null;
            string targetHost = null;
            if (!string.IsNullOrEmpty(_clientOption.Certificate))
            {
                cert = new X509Certificate2(_clientOption.Certificate, _clientOption.CertificatePassword);
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }

            _bootstrap.Handler(new ActionChannelInitializer<IChannel>(c =>
            {
                var pipeline = c.Pipeline;

                if (cert != null)
                {
                    pipeline.AddLast("tls",
                        new TlsHandler(stream =>
                            new SslStream(stream, true,
                                (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                }

                pipeline.AddLast(new LoggingHandler("CLT-CONN"));


                foreach (var kv in _handlerPipeline.BuildPipeline(false))
                {
                    pipeline.AddLast(kv.Key, kv.Value);
                }

                pipeline.AddLast(new TcpClientChannelHandlerAdapter<TMessage>(this));

            }));
        }
        #endregion

        #region Events
        public event EventHandler<MessageReceivedEventArgs<TMessage>> OnReceived;
        public event EventHandler<ErrorEventArgs<TMessage>> OnError;
        public event EventHandler<ConnectedEventArgs<TMessage>> OnConnected;
        public event EventHandler<DisconnectedEventArgs<TMessage>> OnDisconnected;
        public event EventHandler<IdleStateEventArgs<TMessage>> OnIdleState;
        #endregion


        public void RaiseConnected(ISocketContext<TMessage> context)
        {
            OnConnected?.Invoke(this, new ConnectedEventArgs<TMessage>(context));
        }

        public void RaiseDisconnected(ISocketContext<TMessage> context)
        {
            OnDisconnected?.Invoke(this, new DisconnectedEventArgs<TMessage>(context));
        }

        public void RaiseError(ISocketContext<TMessage> context, Exception ex)
        {
            OnError?.Invoke(this, new ErrorEventArgs<TMessage>(context, ex));
        }

        public void RaiseReceive(ISocketContext<TMessage> context, TMessage msg)
        {
            OnReceived?.Invoke(this, new MessageReceivedEventArgs<TMessage>(context, msg));
        }

        public void RaiseIdleState(SocketContext<TMessage> context, IdleStateEvent eventState)
        {
            OnIdleState?.Invoke(this, new IdleStateEventArgs<TMessage>(context));
        }

        #region Methods

        public async Task SendAsync(EndPoint endPoint, TMessage message)
        {
            ISocketContext<TMessage> context = await ConnectAsync(endPoint);
            await context.SendAsync(message);
        }

        public Task<ISocketContext<TMessage>> ConnectAsync(EndPoint endPoint)
        {
            return ConnectAsync(endPoint, true);
        }

        /// <summary>
        /// 如果Cache为False ，调用者需要自行管理ISocketContext
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public async Task<ISocketContext<TMessage>> ConnectAsync(EndPoint endPoint, bool cache = true)
        {
            if (cache)
            {
                if (channels.TryGetValue(endPoint, out var context)
                    && context.Active)
                {
                    return context;
                }
            }

            var channel = await _bootstrap.ConnectAsync(endPoint);
            var newCtx = new SocketContext<TMessage>(channel);

            if (cache)
                channels.AddOrUpdate(endPoint, newCtx, (x, y) => newCtx);

            return newCtx;

        }

        public async Task ShutdownGracefullyAsync(int quietPeriodMS, int shutdownTimeoutMS)
        {
            foreach (var c in channels.Values)
            {
                await c.Channel.CloseAsync();
            }

            channels.Clear();
            await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(quietPeriodMS), TimeSpan.FromMilliseconds(shutdownTimeoutMS));
        }
        #endregion
    }
}
