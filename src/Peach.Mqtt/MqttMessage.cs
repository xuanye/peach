using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Codecs.Mqtt.Packets;
using Peach.Messaging;

namespace Peach.Mqtt
{
    public class MqttMessage : IMessage
    {
        public Packet Packet { get; set; }
    }
}
