using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Peach.Buffer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peach
{
    /// <summary>
    /// 链接上下文的默认实现
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class SocketContext<TMessage> : ISocketContext<TMessage> where TMessage : Messaging.IMessage
    {
        private readonly IChannel _channel;
      
        public SocketContext(IChannel channel)
        {
            _channel = channel;
        }
        public bool Active => _channel.Active;

        public IChannel Channel => _channel;

        public string Id => this._channel.Id.AsLongText();

        public IPEndPoint LocalEndPoint => (IPEndPoint)_channel.LocalAddress;

        public IPEndPoint RemoteEndPoint => (IPEndPoint)_channel.RemoteAddress;

        public Task SendAsync(TMessage message)
        {
            if (_channel.IsWritable)
            {
                if (message != null)
                {
                    return _channel.WriteAndFlushAsync(message);
                }
            }

            return Task.CompletedTask;
        }

        
    }
}
