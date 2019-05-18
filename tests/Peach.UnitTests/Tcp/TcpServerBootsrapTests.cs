using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Peach.Messaging;
using Peach.Protocol;
using Xunit;

namespace Peach.UnitTests.Tcp
{
    public class TcpServerBootsrapTests
    {
        [Fact]
        public async Task RunHostShouldBeNoError()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));


            //协议
            services.AddSingleton(new Mock<IChannelHandlerPipeline>().Object);
            //挂载服务逻辑
            services.AddSingleton(new Mock<ISocketService<CommandLineMessage>>().Object);
            //添加挂载的宿主服务
            services.AddTcpServer<CommandLineMessage>();
            services.AddLogging();

            var host = services.BuildServiceProvider().GetRequiredService<IHostedService>();

            await host.StartAsync(CancellationToken.None);
            await host.StopAsync(CancellationToken.None);
        }
    }
}
