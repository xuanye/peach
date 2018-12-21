using DotNetty.Handlers.Timeout;
using Hey;
using Hey.Config;
using Hey.Messaging;
using Hey.Protocol;
using Hey.Tcp;
using System;
using System.Threading.Tasks;

namespace CommandLine.Client
{
    public class MyCommandClient : TcpClient<CommandLineMessage>
    {
        public MyCommandClient(IProtocol<CommandLineMessage> protocol) : base(protocol)
        {
        }

        public MyCommandClient(TcpClientOption clientOption, IProtocol<CommandLineMessage> protocol) : base(clientOption, protocol)
        {
        }
               

        public override void OnConnected(ISocketContext<CommandLineMessage> context)
        {
            Console.WriteLine("Server {0} Connected ", context.RemoteEndPoint);
            base.OnConnected(context);
        }

        public override void OnDisconnected(ISocketContext<CommandLineMessage> context)
        {
            Console.WriteLine("Server {0} DisConnected ", context.RemoteEndPoint);
            base.OnDisconnected(context);
        }

        public override void OnException(ISocketContext<CommandLineMessage> context, Exception ex)
        {
            Console.WriteLine("Occ Error  {0} \r\n ===================\r\n {1}",ex.Message, ex.StackTrace);
            base.OnException(context, ex);
        }

        public override void OnRecieve(ISocketContext<CommandLineMessage> context, CommandLineMessage msg)
        {
            string content = string.Format("{0} {1}", msg.Command, string.Join(" ", msg.Parameters));
            Console.WriteLine("recieve message {0} from {1}", content,context.RemoteEndPoint);
            base.OnRecieve(context, msg);
        }

        public override void OnIdleState(SocketContext<CommandLineMessage> context, IdleStateEvent eventState)
        {
            Task.Run(async () =>
            {
                CommandLineMessage heartBeat = new CommandLineMessage("heartbeat");
                await context.SendAsync(heartBeat).ConfigureAwait(false);
            });
           
            base.OnIdleState(context, eventState);
        }
    }
}