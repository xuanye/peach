using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace Peach.Protocol
{
    public class CommandLineChannelHandlerPipeline : IChannelHandlerPipeline
    {

        public Dictionary<string, IChannelHandler> BuildPipeline(bool isServer)
        {
            int timeout = isServer ? 30 : 15;
            return new Dictionary<string, IChannelHandler> {
                { "timeout", new IdleStateHandler(0, 0, timeout) }, // heartbeat each 30 second             
                { "framing-enc", new LengthFieldPrepender(2) },
                { "message-enc", new CommandLineEncodeHandler() },
                { "framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2) },
                { "message-dec", new CommandLineDecodeHandler() }
            };
        }
    }
}
