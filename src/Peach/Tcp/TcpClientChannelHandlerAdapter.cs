using System;
using System.Diagnostics;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Peach.Diagnostics;

namespace Peach.Tcp
{
    public class TcpClientChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage : Messaging.IMessage
    {
        private static DiagnosticListener listener = new DiagnosticListener(Diagnostics.DiagnosticListenerExtensions.DiagnosticListenerName);

        private readonly ISocketClient<TMessage> _client;

        public TcpClientChannelHandlerAdapter(ISocketClient<TMessage> client)
        {
            _client = client;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _client.RaiseConnected(new SocketContext<TMessage>(context.Channel));
            base.ChannelActive(context);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _client.RaiseDisconnected(new SocketContext<TMessage>(context.Channel));
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            listener.ClientReceive(msg);
            _client.RaiseReceive(new SocketContext<TMessage>(context.Channel), msg);
            listener.ClientReceiveComplete(msg);
            //this._bootstrap.ChannelRead(ctx, msg);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            _client.RaiseError(new SocketContext<TMessage>(context.Channel), ex);
            listener.ClientException(ex);
            context.CloseAsync(); //关闭连接
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                var eventState = evt as IdleStateEvent;
                if (eventState != null)
                {
                    _client.RaiseIdleState(new SocketContext<TMessage>(context.Channel), eventState);
                }
            }
        }
    }
}