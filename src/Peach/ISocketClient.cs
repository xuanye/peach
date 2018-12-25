using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Handlers.Timeout;
using Peach.Messaging;

namespace Peach
{
    public interface ISocketClient<TMessage> where TMessage : IMessage
    {
        void OnRecieve(ISocketContext<TMessage> context, TMessage msg);

        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="context"></param>
        void OnConnected(ISocketContext<TMessage> context);

        /// <summary>
        /// 当socket连接断开时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnDisconnected(ISocketContext<TMessage> context);

        /// <summary>
        /// 当发生异常时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnException(ISocketContext<TMessage> context, Exception ex);

        /// <summary>
        /// 需要发送心跳包
        /// </summary>
        /// <param name="context"></param>
        /// <param name="eventState"></param>
        void OnIdleState(SocketContext<TMessage> context, IdleStateEvent eventState);
    }
}