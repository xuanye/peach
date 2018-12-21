using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hey.Messaging;

namespace Hey
{
    /// <summary>
    /// TODO:命名
    /// </summary>
    public interface ISocketService<TMessage> where TMessage : IMessage
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
    }
}
