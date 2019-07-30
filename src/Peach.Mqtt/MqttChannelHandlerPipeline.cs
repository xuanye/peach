// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using DotNetty.Codecs.Mqtt;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Options;

namespace Peach.Mqtt
{
    public class MqttChannelHandlerPipeline : IChannelHandlerPipeline
    {
        readonly MqttOptions _options;

        public MqttChannelHandlerPipeline(IOptions<MqttOptions> optionsAccessor) : this(optionsAccessor?.Value)
        {
        }

        public MqttChannelHandlerPipeline(MqttOptions options)
        {
            _options = options ?? new MqttOptions();
        }

        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {
            //注意编码顺序和解码顺序刚好是相反的 
            return new Dictionary<string, IChannelHandler> {
                { "mqtt-enc", new MqttEncoder()},
                { "message-enc", new MqttMessageEncodeHandler()},
                
                { "mqtt-dec", new MqttDecoder(isServer, _options.MaxMessageSize) },
                { "message-dec", new MqttMessageDecodeHandler()}
            };
        }
    }
}