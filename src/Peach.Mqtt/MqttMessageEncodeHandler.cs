// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
