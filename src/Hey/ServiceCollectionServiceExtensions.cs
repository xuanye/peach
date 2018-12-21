using Hey.Config;
using Hey.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hey
{
    public static class ServiceCollectionServiceExtensions
    {       
        public static IServiceCollection AddTcpServer<TMessage>(this IServiceCollection services) 
            where TMessage:Messaging.IMessage
        {
            return services
                .AddSingleton<IServerBootstrap,Tcp.TcpServerBootstrap<TMessage>>()
                .AddScoped<IHostedService, HeyHostedService>();
        }
    }
}
