// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using DotNetty.Codecs.Mqtt.Packets;

    public class MqttSubscription
    {
        public string Topic { get; set; }
        
        public QualityOfService Qos { get; set; }
        public MqttClientSession ClientSession { get; set; }
        
    }
}