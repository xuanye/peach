// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public class PingReqPacketProcessor:IPacketProcessor
    {
        static readonly MqttMessage PingAckMsg = new MqttMessage{ Packet =  PingRespPacket.Instance};
        public PacketType PacketType => PacketType.PINGREQ;

        public Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet)
        {
            return Task.FromResult(PingAckMsg);
        }
    }
}