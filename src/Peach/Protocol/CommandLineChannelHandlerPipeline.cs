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
            return new Dictionary<string, IChannelHandler> {
                { "timeout", new IdleStateHandler(0, 0, 360) }, // heartbeat each 6 second             
                { "framing-enc", new LengthFieldPrepender(2) },
                { "message-enc", new CommandLineEncodeHandler() },
                { "framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2) },
                { "message-dec", new CommandLineDecodeHandler() }
            };
        }
    }
}
