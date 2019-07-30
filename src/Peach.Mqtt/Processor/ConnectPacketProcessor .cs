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
    public class ConnectPacketProcessor:IPacketProcessor
    {
        readonly IMqttAuthorize mqttAuthorize;
        readonly ILogger<ConnectPacketProcessor> logger;

        public ConnectPacketProcessor(IMqttAuthorize mqttAuthorize, ILogger<ConnectPacketProcessor> logger)
        {
            this.mqttAuthorize = mqttAuthorize;
            this.logger = logger;
        }
        public PacketType PacketType => PacketType.CONNECT;

        public async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet)
        {
         
            var ack  = new ConnAckPacket();
            var resultMsg = new MqttMessage { Packet =  ack} ;
          
            if (!(packet is ConnectPacket cntPacket))
            {
                this.logger.LogWarning("bad data format");
                ack.ReturnCode = ConnectReturnCode.RefusedNotAuthorized;
                return resultMsg;
            }
            this.logger.LogWarning("receive connect packet ,clientId={0}",cntPacket.ClientId);
            IMqttResult validResult = await this.mqttAuthorize.Validate(cntPacket);
            
            ack.ReturnCode = validResult.Code == 0 ? ConnectReturnCode.Accepted : ConnectReturnCode.RefusedNotAuthorized;

            return resultMsg;

        }
    }
}