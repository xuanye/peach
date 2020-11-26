// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Peach.Mqtt.Processor;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqtt(this IServiceCollection services)
        {
            
            //默认认证方式
            services.TryAddSingleton<IMqttAuthorize,NoopMqttAuthorize>();
            
            services.AddPacketProcessor()
                .AddSingleton<PacketProcessorManager>()
            .AddSingleton<MqttClientSessionManager>();
            
            return services;
        }

        static IServiceCollection AddPacketProcessor(this IServiceCollection services)
        {
            return services.AddSingleton<IPacketProcessor,ConnectPacketProcessor>()
                .AddSingleton<IPacketProcessor, DisConnectPacketProcessor>()
                //.AddSingleton<IPacketProcessor, PingReqPacketProcessor>()
                
                //.AddSingleton<IPacketProcessor, PublishPacketProcessor>()
                //.AddSingleton<IPacketProcessor, PubAckPacketProcessor>()
                //.AddSingleton<IPacketProcessor, PubCompPacketProcessor>()
                //.AddSingleton<IPacketProcessor, PubRecPacketProcessor>()
                //.AddSingleton<IPacketProcessor, PubRelPacketProcessor>()
                
                //.AddSingleton<IPacketProcessor, SubscribePacketProcessor>()
                //.AddSingleton<IPacketProcessor, UnSubscribePacketProcessor>()
               ;
        }
    }
}