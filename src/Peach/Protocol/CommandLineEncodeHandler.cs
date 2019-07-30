using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Peach.Messaging;

namespace Peach.Protocol
{
    public class CommandLineEncodeHandler : DotNetty.Codecs.MessageToByteEncoder<CommandLineMessage>
    {
        protected override void Encode(IChannelHandlerContext context, CommandLineMessage message, IByteBuffer output)
        {
            string content = $"{message.Command}{CommandLineProtocol.SPLITER}{string.Join(CommandLineProtocol.SPLITER, message.Parameters)}";
            output.WriteBytes(Encoding.UTF8.GetBytes(content));          
        }
    }
}
