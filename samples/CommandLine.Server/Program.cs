using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Peach.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Peach;
using Peach.Messaging;
using Peach.Protocol;
using Peach.Tcp;
using Peach.Config;

namespace CommandLine.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                /*
                services.Configure<TcpHostOption>(o =>
                {
                    o.Certificate = Path.Combine(AppContext.BaseDirectory,"../../../../../shared/dotnetty.com.pfx");
                    o.CertificatePassword = "password";
                });
                */
                //协议
                services.AddSingleton<IChannelHandlerPipeline, CommandLineChannelHandlerPipeline>();
                //挂载服务逻辑
                services.AddSingleton<ISocketService<CommandLineMessage>, MyService>();

                //添加挂载的宿主服务
                services.AddTcpServer<CommandLineMessage>();
            })
            .ConfigureLogging(
                logger =>
                {
                    logger.AddConsole();
                }
            );

            builder.RunConsoleAsync().Wait();
        }
    }
}
