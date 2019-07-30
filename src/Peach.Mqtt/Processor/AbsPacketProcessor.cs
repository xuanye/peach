// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt.Processor
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public abstract class AbsPacketProcessor<TPacket>:IPacketProcessor
        where TPacket : Packet
    {
        public abstract PacketType PacketType { get; }

        public async Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, Packet packet)
        {
            if (!(packet is TPacket target))
            {
                return MqttMessage.BAD_DATA_FORMAT;
            }

            return await  this.ProcessAsync(clientSession, target);

        }

        protected abstract Task<MqttMessage> ProcessAsync(MqttClientSession clientSession, TPacket packet);


    }
}