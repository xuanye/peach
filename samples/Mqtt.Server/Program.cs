using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Peach;


namespace Mqtt.Server
{
    using Peach.Mqtt;

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
                    services.AddSingleton<IChannelHandlerPipeline, MqttChannelHandlerPipeline>();
                    //挂载服务逻辑
                    services.AddSingleton<ISocketService<MqttMessage>, MqttSampleService>();

                    services.AddMqtt();
                   
                    //添加挂载的宿主服务
                    services.AddTcpServer<MqttMessage>();
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