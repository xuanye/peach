// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 发布主题
    /// </summary>
    public class PublishPacketProcessor:AbsPacketProcessor<PublishPacket>
    {
        readonly MqttSubscriptionManager subscriptionManager;
        readonly ILogger<PublishPacketProcessor> logger;

        public PublishPacketProcessor(MqttSubscriptionManager subscriptionManager,ILogger<PublishPacketProcessor> logger)
        {
            this.subscriptionManager = subscriptionManager;
            this.logger = logger;
        }

        public override PacketType PacketType => PacketType.PUBLISH;

        protected override async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, PublishPacket packet)
        {
            List<string> clientIds = await subscriptionManager.GetSubscribeClientIds(packet.TopicName);
            if(clientIds !=null && clientIds.Count > 0)
            {
                //循环依次发送信息
            }

            return MqttMessage.SUCCESS;
        }
    }
}