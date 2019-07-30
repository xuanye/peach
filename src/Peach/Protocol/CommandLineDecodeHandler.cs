using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Peach.Messaging;

namespace Peach.Protocol
{
    public class CommandLineDecodeHandler : DotNetty.Codecs.ByteToMessageDecoder
    {
      
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            output.Add(GetCommandLineMessage(input));
        }


        private CommandLineMessage GetCommandLineMessage(IByteBuffer input)
        {
            byte[] buffer = new byte[input.ReadableBytes];
            input.ReadBytes(buffer);
            string content = Encoding.UTF8.GetString(buffer);

            var arr = content.Split(new string[] { CommandLineProtocol.SPLITER }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length == 0)
                return new CommandLineMessage(string.Empty);
            if (arr.Length == 1)
                return new CommandLineMessage(arr[0]);

            return new CommandLineMessage(arr[0], arr.Skip(1).ToArray());
        }
    }


}
