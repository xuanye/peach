// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    public class SubscribePacketProcessor:IPacketProcessor
    {
        readonly MqttSubscriptionManager subscriptionManager;
        readonly ILogger<SubscribePacketProcessor> logger;

        public SubscribePacketProcessor(MqttSubscriptionManager subscriptionManager,ILogger<SubscribePacketProcessor> logger)
        {
            this.subscriptionManager = subscriptionManager;
            this.logger = logger;
        }
        public PacketType PacketType => PacketType.SUBSCRIBE;

        public async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet)
        {
            if (!(packet is SubscribePacket reqPacket))
            {
                this.logger.LogWarning("bad data format");
                return null;
            }

            IMqttResult result =  await subscriptionManager.Subscribe(clientSession, reqPacket);
            //reqPacket.Requests[0].TopicFilter
            
            return new MqttMessage { Code =  result.Code};
            
        }
    }
}