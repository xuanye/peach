using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Moq;
using Peach.Messaging;
using Peach.Tcp;
using Xunit;

namespace Peach.UnitTests.Tcp
{
    public class TcpServerChannelHandlerAdapterTests
    {

        [Fact]
        public void TestExceptionCaught()
        {
          
            var socketService = new Mock<ISocketService<CommandLineMessage>>();

            var handler = new TcpServerChannelHandlerAdapter<CommandLineMessage>(socketService.Object);

            var handlerCtx = new Mock<IChannelHandlerContext>();

            handler.ChannelReadComplete(handlerCtx.Object);
            handler.ExceptionCaught(handlerCtx.Object, new Exception("test"));
        }

    }
}
