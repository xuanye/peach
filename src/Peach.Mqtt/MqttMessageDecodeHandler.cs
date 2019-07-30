// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
