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
    public class TcpClient<TMessage>:ISocketClient<TMessage> where TMessage:Messaging.IMessage
    {

        private readonly Bootstrap _bootstrap = new Bootstrap();
        private readonly MultithreadEventLoopGroup _group = new MultithreadEventLoopGroup();
        private readonly IProtocol<TMessage> _protocol;

        private readonly TcpClientOption _clientOption;

        private readonly ConcurrentDictionary<EndPoint, SocketContext<TMessage>> channels
            = new ConcurrentDictionary<EndPoint, SocketContext<TMessage>>();


        public TcpClient(IOptions<TcpClientOption> clientOption,IProtocol<TMessage> protocol)
            :this(clientOption.Value, protocol)
        {

        }
        public TcpClient(IProtocol<TMessage> protocol)
            : this(new TcpClientOption(), protocol)
        {

        }

        public TcpClient(TcpClientOption clientOption, IProtocol<TMessage> protocol)
        {
            this._clientOption = clientOption;
            this._protocol = protocol;
            InitBootstrap();
        }


        #region Init

        /// <summary>
        /// Init Bootstrap
        /// </summary>
        private void InitBootstrap()
        {
            this._bootstrap.Group(this._group)
                .Channel<TcpSocketChannel>();

            if (this._clientOption.TcpNodelay)
            {
                this._bootstrap.Option(ChannelOption.TcpNodelay, true);
            }
            if (this._clientOption.SoKeepalive)
            {
                this._bootstrap.Option(ChannelOption.SoKeepalive, true);
            }
            if (this._clientOption.ConnectTimeout > 0)
            {
                this._bootstrap.Option(ChannelOption.ConnectTimeout, TimeSpan.FromMilliseconds(this._clientOption.ConnectTimeout));
            }

            X509Certificate2 cert = null;
            string targetHost = null;
            if (!string.IsNullOrEmpty(this._clientOption.Certificate))
            {
                cert = new X509Certificate2(this._clientOption.Certificate, this._clientOption.CertificatePassword);
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }
            
            this._bootstrap.Handler(new ActionChannelInitializer<IChannel>(c =>
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
                
                
             
                var meta = this._protocol.GetProtocolMeta();

                if (meta != null)
                {
                    // IdleStateHandler
                    pipeline.AddLast("idle", new IdleStateHandler(0, 0, meta.HeartbeatInterval / 1000 )); 

                    //消息前处理
                    pipeline.AddLast(
                        new LengthFieldBasedFrameDecoder(
                            meta.MaxFrameLength,
                            meta.LengthFieldOffset,
                            meta.LengthFieldLength,
                            meta.LengthAdjustment,
                            meta.InitialBytesToStrip
                        )
                    );
                }
                else //Simple Protocol For Test
                {
                    pipeline.AddLast("idle", new IdleStateHandler(0, 0, 360)); // heartbeat each 6 second
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                }


                pipeline.AddLast(new ChannelDecodeHandler<TMessage>(this._protocol));
                pipeline.AddLast(new TcpClientChannelHandlerAdapter<TMessage>(this, this._protocol));

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
            var context = await ConnectAsync(endPoint);
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
        public async Task<ISocketContext<TMessage>> ConnectAsync(EndPoint endPoint,bool cache= true)
        {
            if (cache)
            {
                if (this.channels.TryGetValue(endPoint, out var context)
                    && context.Active)
                {
                    return context;
                }
            }
            
            var channel = await this._bootstrap.ConnectAsync(endPoint);
            var newCtx = new SocketContext<TMessage>(channel, this._protocol);
            
            if(cache)
                this.channels.AddOrUpdate(endPoint, newCtx, (x, y) => newCtx);
            
            return newCtx;
            
        }

        public async Task ShutdownGracefullyAsync(int quietPeriodMS, int shutdownTimeoutMS)
        {
            foreach(var c in this.channels.Values)
            {
               await c.Channel.CloseAsync();
            }

            this.channels.Clear();
            await this._group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(quietPeriodMS), TimeSpan.FromMilliseconds(shutdownTimeoutMS));
        }
        #endregion
    }
}
