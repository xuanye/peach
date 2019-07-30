using System;
using System.Diagnostics;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Peach.Diagnostics;

namespace Peach.Tcp
{
    public class TcpServerChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage : Messaging.IMessage
    {
        static DiagnosticListener listener = new DiagnosticListener(Diagnostics.DiagnosticListenerExtensions.DiagnosticListenerName);
        readonly ISocketService<TMessage> _service;

        public TcpServerChannelHandlerAdapter(
            ISocketService<TMessage> service
        ) : base(true)
        {
            _service = service;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _service.OnConnected(new SocketContext<TMessage>(context.Channel));
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _service.OnDisconnected(new SocketContext<TMessage>(context.Channel));
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            listener.ServiceReceive(msg);
            _service.OnReceive(new SocketContext<TMessage>(context.Channel), msg);
            listener.ServiceReceiveCompleted(msg);
        }

        public override void ChannelReadComplete(IChannelHandlerContext contex)
        {
            contex.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            _service.OnException(new SocketContext<TMessage>(context.Channel), ex);
            listener.ServiceException(ex);
            context.CloseAsync(); //关闭连接
        }

        //服务端超时则直接关闭链接
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                var eventState = evt as IdleStateEvent;
                if (eventState != null)
                {
                    context.CloseAsync(); //关闭连接
                }
            }
        }
    }
}