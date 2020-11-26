// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    public class UnSubscribePacketProcessor:AbsPacketProcessor<UnsubscribePacket>
    {
      
        readonly ILogger<UnSubscribePacketProcessor> _logger;

        public UnSubscribePacketProcessor(ILogger<UnSubscribePacketProcessor> logger)
        {

            _logger = logger;
        }
        public override PacketType PacketType => PacketType.UNSUBSCRIBE;

        protected override Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, UnsubscribePacket packet)
        {
            //TODO:处理取消订阅逻辑和转发请求
            _logger.LogDebug("recieve UnsubscribePacket message");
            return  Task.FromResult(MqttMessage.SUCCESS);
        }
    }
}