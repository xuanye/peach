// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    /// <summary>
    /// 断开连接的处理器
    /// </summary>
    public class DisConnectPacketProcessor:AbsPacketProcessor<DisconnectPacket>
    {
        public override PacketType PacketType => PacketType.DISCONNECT;

        protected override Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, DisconnectPacket packet)
        {
            MqttMessage message = new MqttMessage { Code = -1 };

            //TODO: 处理一些断开连接需要完成的任务

            return Task.FromResult(message);
        }
       
    }
}