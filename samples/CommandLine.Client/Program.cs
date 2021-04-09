using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Peach.Config;
using Peach.Infrastructure;
using Peach.Messaging;
using Peach.Protocol;
using Peach.Tcp;

namespace CommandLine.Client
{
    static class Program
    {
        static void Main(string[] args)
        {
            /*
            TcpClientOption option = new TcpClientOption {
                Certificate = Path.Combine(AppContext.BaseDirectory, "../../../../../shared/dotnetty.com.pfx"),
                CertificatePassword = "password"
            };
            */
            //实例化Client 需要传入使用的协议
            //TcpClient<CommandLineMessage> client = new TcpClient<CommandLineMessage>(option,new CommandLineChannelHandlerPipeline());
            TcpClient<CommandLineMessage> client = new TcpClient<CommandLineMessage>(new CommandLineChannelHandlerPipeline());
            client.OnReceived += Client_OnReceived;
            client.OnConnected += Client_OnConnected;
            client.OnIdleState += Client_OnIdleState;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnError += Client_OnError;
            Task.Run(async () =>
            {
                //连接服务器，可以链接多个哦
                var socketContext = await client.ConnectAsync(new IPEndPoint(IPUtility.GetLocalIntranetIP(), 5566));

                //发送消息
                Console.WriteLine("send msg init");
                var initCmd = new CommandLineMessage("init");
                await socketContext.SendAsync(initCmd);

                Console.WriteLine("send msg echo hello");
                //发送消息2
                var echoCmd = new CommandLineMessage("echo", "hello");
                await socketContext.SendAsync(echoCmd);


                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
                //关闭链接
                await client.ShutdownGracefullyAsync(2000, 2000);

            }).Wait();

        }

        private static void Client_OnError(object sender, Peach.EventArgs.ErrorEventArgs<CommandLineMessage> e)
        {
            Console.WriteLine("server error,{0}",e.Error.Message);
        }

        private static void Client_OnDisconnected(object sender, Peach.EventArgs.DisconnectedEventArgs<CommandLineMessage> e)
        {
            Console.WriteLine("server is disconnected");           
        }

        private static void Client_OnIdleState(object sender, Peach.EventArgs.IdleStateEventArgs<CommandLineMessage> e)
        {


           Task.Run(async () =>
           {
               Console.WriteLine("send idel cmd");
               var idelCmd = new CommandLineMessage("idle");
               await e.Context.SendAsync(idelCmd);
           });
            
           
        }

        static void Client_OnConnected(object sender, Peach.EventArgs.ConnectedEventArgs<CommandLineMessage> e)
        {
            Console.WriteLine("server is connected");
        }

        static void Client_OnReceived(object sender, Peach.EventArgs.MessageReceivedEventArgs<CommandLineMessage> e)
        {
            string content = $"{e.Message.Command} {string.Join(" ", e.Message.Parameters)}";
            Console.WriteLine("receive message {0} from {1}", content, e.Context.RemoteEndPoint);
        }
    }
}
