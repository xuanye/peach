using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hey
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketContext<TMessage> where TMessage : Messaging.IMessage
    {
        
  
        /// <summary>
        /// get the connection id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Local Address
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Remote Address
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        IChannel Channel { get; }


        bool Active { get; }

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAsync(TMessage message);
    }
}
