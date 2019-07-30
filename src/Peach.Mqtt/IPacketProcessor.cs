// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public interface IPacketProcessor
    {
        PacketType PacketType { get; }
        
        Task<MqttMessage> ProcessAsync(MqttClientSession clientSession,Packet packet);
    }
}