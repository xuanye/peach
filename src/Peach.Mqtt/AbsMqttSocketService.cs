// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System;
    using DotNetty.Codecs.Mqtt.Packets;

    public abstract class AbsMqttSocketService:AbsSocketService<MqttMessage>
    {
        public override void OnReceive(ISocketContext<MqttMessage> context, MqttMessage msg)
        {
            Console.WriteLine("receive Packet from {0}",context.RemoteEndPoint);
            if (msg?.Packet == null)
            {
                return;
            }
            Console.WriteLine("receive Packet from {0}, type ={1}",context.RemoteEndPoint,msg.Packet.PacketType);
            switch (msg.Packet.PacketType)
            {
                case  PacketType.CONNECT:
                    Console.WriteLine("receive connect pack client_id = {0}",((ConnectPacket)msg.Packet).ClientId);
                    var ack = new ConnAckPacket { ReturnCode = ConnectReturnCode.Accepted, SessionPresent = true};
                    context.SendAsync(new MqttMessage { Packet =  ack});
                    break;
                default:
                    Console.WriteLine("receive Packet from {0}, type ={1}",context.RemoteEndPoint,msg.Packet.PacketType);
                    break;
                
            }
           
        }
    }
}