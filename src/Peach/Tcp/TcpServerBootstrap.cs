using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Handlers.Tls;
using Peach.Config;
using Peach.Infrastructure;

namespace Peach.Tcp
{
    /// <inheritdoc />
    /// <summary>
    /// Tcp Server
    /// </summary>
    public class TcpServerBootstrap<TMessage> : IServerBootstrap where TMessage : Messaging.IMessage
    {
        private readonly TcpHostOption _options;        
        private readonly ISocketService<TMessage> _socketService;
        private readonly IChannelHandlerPipeline _handlerPipeline;
        private readonly ILogger _logger;

        private IChannel _channel;
    
        private IEventLoopGroup _bossGroup;
        private IEventLoopGroup _workerGroup;

        public TcpServerBootstrap(
                ISocketService<TMessage> socketService,
                IChannelHandlerPipeline handlerPipeline,
                ILoggerFactory loggerFactory,
                IOptions<TcpHostOption> hostOption = null
            )
        {

            Preconditions.CheckNotNull(handlerPipeline, nameof(handlerPipeline));
            Preconditions.CheckNotNull(socketService, nameof(socketService));

            _options = hostOption?.Value ?? new TcpHostOption();
            _socketService = socketService;
            _handlerPipeline = handlerPipeline;
            _logger = loggerFactory.CreateLogger(GetType());
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            }


            //if (_options.UseLibuv)
            //{
            //    var dispatcher =  new DispatcherEventLoopGroup();
            //    _bossGroup = dispatcher;             
            //    _workerGroup = new WorkerEventLoopGroup(dispatcher);
            //}
            //else
            {
                // 主的线程
                _bossGroup = new MultithreadEventLoopGroup(1);
                // 工作线程，默认根据CPU计算
                _workerGroup = new MultithreadEventLoopGroup();
            }

            var bootstrap = new ServerBootstrap()
                .Group(_bossGroup, _workerGroup);

            //if (_options.UseLibuv)
            {
               //bootstrap.Channel<TcpServerChannel>();
            }
            //else
            {
                bootstrap.Channel<TcpServerSocketChannel>();
            }

            bootstrap.Option(ChannelOption.SoBacklog, _options.SoBacklog); //NOTE: 是否可以公开更多Netty的参数

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }

            X509Certificate2 tlsCertificate = null;

            if (!string.IsNullOrEmpty(_options.Certificate))
            {
                tlsCertificate = new X509Certificate2(_options.Certificate, _options.CertificatePassword);
            }

            _ = bootstrap.Handler(new LoggingHandler("LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;


                    if (tlsCertificate != null)
                    {
                        pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
                    }

                    pipeline.AddLast(new LoggingHandler("CONN"));

                    foreach (var kv in _handlerPipeline.BuildPipeline(true))
                    {
                        pipeline.AddLast(kv.Key, kv.Value);
                    }

                    //业务处理Handler，即解码成功后如何处理消息的类
                    pipeline.AddLast(new TcpServerChannelHandlerAdapter<TMessage>(_socketService));
                }));

            if (_options.BindType == AddressBindType.Any)
            {
                _channel = await bootstrap.BindAsync(_options.Port);
            }
            else if (_options.BindType == AddressBindType.InternalAddress)
            {
                var localPoint = IPUtility.GetLocalIntranetIP();
                if(localPoint == null)
                {
                    this._logger.LogWarning("there isn't an avaliable internal ip address,the service will be hosted at loopback address.");
                    _channel = await bootstrap.BindAsync(IPAddress.Loopback, _options.Port);
                }
                else
                {
                    //this._logger.LogInformation("TcpServerHost bind at {0}",localPoint);
                    _channel = await bootstrap.BindAsync(localPoint, this._options.Port);

                }
                
            }
            else if (_options.BindType == AddressBindType.Loopback)
            {
                _channel = await bootstrap.BindAsync(IPAddress.Loopback, _options.Port);
            }
            else
            {
                _channel = await bootstrap.BindAsync(IPAddress.Parse(_options.SpecialAddress), _options.Port);
            }

            Console.Write(_options.StartupWords, _channel.LocalAddress);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TcpServerHost is stopping...");
            await _channel.CloseAsync();
            var quietPeriod = TimeSpan.FromMilliseconds(_options.QuietPeriod);
            var shutdownTimeout = TimeSpan.FromMilliseconds(_options.ShutdownTimeout);
            await _workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            await _bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            _logger.LogInformation("TcpServerHost is stopped!");
            //NOTE:Close Client?
        }
    }
}
