// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    public class UnSubscribePacketProcessor:IPacketProcessor
    {
        readonly MqttSubscriptionManager subscriptionManager;
        readonly ILogger<UnSubscribePacketProcessor> logger;

        public UnSubscribePacketProcessor(MqttSubscriptionManager subscriptionManager,ILogger<UnSubscribePacketProcessor> logger)
        {
            this.subscriptionManager = subscriptionManager;
            this.logger = logger;
        }
        public PacketType PacketType => PacketType.UNSUBSCRIBE;

        public async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet)
        {
            if (!(packet is UnsubscribePacket reqPacket))
            {
                this.logger.LogWarning("bad data format");
                return null;
            }

            IMqttResult result = await this.subscriptionManager.UnSubscribe(clientSession, reqPacket);
            
            return new MqttMessage { Code =  result.Code};
        }
    }
}