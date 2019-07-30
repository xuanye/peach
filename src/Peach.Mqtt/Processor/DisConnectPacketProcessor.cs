// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public class DisConnectPacketProcessor:IPacketProcessor
    {
        public PacketType PacketType => PacketType.DISCONNECT;

        public Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet)
        {
            MqttMessage message = new MqttMessage{ Code =  -1 };
            
            //TODO: 处理一些断开连接需要完成的任务
            
            return Task.FromResult(message);
        }
    }
}