// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 连接包处理器
    /// </summary>
    public class ConnectPacketProcessor:AbsPacketProcessor<ConnectPacket>
    {
        readonly IMqttAuthorize _mqttAuthorize;
        readonly ILogger<ConnectPacketProcessor> _logger;

        public ConnectPacketProcessor(IMqttAuthorize mqttAuthorize, ILogger<ConnectPacketProcessor> logger)
        {
            _mqttAuthorize = mqttAuthorize;
            _logger = logger;
        }
        public override PacketType PacketType => PacketType.CONNECT;


        protected override async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, ConnectPacket packet)
        {
            var ack = new ConnAckPacket();
            var resultMsg = new MqttMessage { Packet = ack };

            _logger.LogInformation("receive connect packet ,clientId={0}", packet.ClientId);
            IMqttResult validResult = await _mqttAuthorize.Validate(packet);

            ack.ReturnCode = validResult.Code == 0 ? ConnectReturnCode.Accepted : ConnectReturnCode.RefusedNotAuthorized;

            return resultMsg;
        }      
    }
}