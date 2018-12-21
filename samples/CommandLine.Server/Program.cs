using Microsoft.Extensions.Hosting;
using System;
using Hey.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hey;
using Hey.Messaging;
using Hey.Protocol;
using Hey.Tcp;
using Hey.Config;

namespace CommandLine.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()          
            .ConfigureServices((context,services) =>
            {
                //协议
                services.AddSingleton<IProtocol<CommandLineMessage>, CommandLineProtocol>();
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
