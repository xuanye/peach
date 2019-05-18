using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;

namespace Peach
{
    public interface IChannelHandlerPipeline
    {
        Dictionary<string, IChannelHandler> BuildPipeline(bool isServer);
    }
}
