using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Codecs;
using DotNetty.Codecs.Mqtt.Packets;
using DotNetty.Transport.Channels;

namespace Peach.Mqtt
{
    public class MqttMessageDecodeHandler : MessageToMessageDecoder<Packet>
    {
        protected override void Decode(IChannelHandlerContext context, Packet packet, List<object> output)
        {
            output.Add(new MqttMessage { Packet = packet });
        }
    }
}
