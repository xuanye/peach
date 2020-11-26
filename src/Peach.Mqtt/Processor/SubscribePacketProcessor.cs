// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    public class SubscribePacketProcessor:AbsPacketProcessor<SubscribePacket>
    {       
        readonly ILogger<SubscribePacketProcessor> _logger;

        public SubscribePacketProcessor(ILogger<SubscribePacketProcessor> logger)
        {

            _logger = logger;
        }
        public override PacketType PacketType => PacketType.SUBSCRIBE;

        protected override Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, SubscribePacket packet)
        {
            //TODO:处理订阅话题的逻辑和转发请求
            _logger.LogDebug("recieve SubscribePacket message");
            return Task.FromResult(MqttMessage.SUCCESS);

        }
    }
}