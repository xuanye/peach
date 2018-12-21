using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hey.Tcp
{   
    public class TcpClientChannelHandlerAdapter<TMessage> : SimpleChannelInboundHandler<TMessage> where TMessage : Messaging.IMessage
    {
        private readonly ISocketClient<TMessage> _client;
        private readonly Protocol.IProtocol<TMessage> _protocol;

        public TcpClientChannelHandlerAdapter(ISocketClient<TMessage> client,Protocol.IProtocol<TMessage> protocol)
        {
            this._client = client;
            this._protocol = protocol;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            this._client.OnConnected(new SocketContext<TMessage>(context.Channel, this._protocol));
            base.ChannelActive(context);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            this._client.OnDisconnected(new SocketContext<TMessage>(context.Channel, this._protocol));
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TMessage msg)
        {
            this._client.OnRecieve(new SocketContext<TMessage>(context.Channel, this._protocol), msg);
            //this._bootstrap.ChannelRead(ctx, msg);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            this._client.OnException(new SocketContext<TMessage>(context.Channel, this._protocol), ex);
            context.CloseAsync(); //关闭连接
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                var eventState = evt as IdleStateEvent;
                if (eventState != null)
                {                  
                    this._client.OnIdleState(new SocketContext<TMessage>(context.Channel, this._protocol), eventState);
                }
            }
        }
    }
}
