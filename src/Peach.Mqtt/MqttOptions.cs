using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Mqtt
{
    public class MqttOptions
    {
        public int MaxMessageSize { get; set; } = 256 * 1024;
    }
}
