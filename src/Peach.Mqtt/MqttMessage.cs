// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DotNetty.Codecs.Mqtt.Packets;
using Peach.Messaging;

namespace Peach.Mqtt
{
    public class MqttMessage : IMessage
    {
        public static readonly MqttMessage BAD_DATA_FORMAT = new MqttMessage { Code = MqttErrorCodes.BAD_DATA_FORMAT };

        public static readonly MqttMessage SUCCESS = new MqttMessage { Code = 0 };
        
        public int Code { get; set; }
        public Packet Packet { get; set; }
    }
}
