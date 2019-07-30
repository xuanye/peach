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
                .AddSingleton<PacketProcessorManager>();
            return services;
        }

        static IServiceCollection AddPacketProcessor(this IServiceCollection services)
        {
            return services.AddSingleton<ConnectPacketProcessor>()
                .AddSingleton<DisConnectPacketProcessor>()
                .AddSingleton<PingReqPacketProcessor>()
                
                .AddSingleton<PublishPacketProcessor>()
                .AddSingleton<PubAckPacketProcessor>()
                .AddSingleton<PubCompPacketProcessor>()
                .AddSingleton<PubRecPacketProcessor>()
                .AddSingleton<PubRelPacketProcessor>()
                
                .AddSingleton<SubscribePacketProcessor>()
                .AddSingleton<UnSubscribePacketProcessor>()
               ;
        }
    }
}