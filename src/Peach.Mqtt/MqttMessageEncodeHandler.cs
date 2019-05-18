using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Peach.Mqtt
{
    public class MqttMessageEncodeHandler : MessageToMessageEncoder<MqttMessage>
    {
        protected override void Encode(IChannelHandlerContext context, MqttMessage message, List<object> output)
        {
            if(message?.Packet != null)
                output.Add(message.Packet);
        }
    }
}
