using System;
using System.Net;
using System.Threading.Tasks;
using Peach.Infrastructure;

namespace CommandLine.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //实例化Client 需要传入使用的协议
            MyCommandClient client = new MyCommandClient(new Peach.Protocol.CommandLineProtocol());

            Task.Run(async () =>
            {
                //连接服务器，可以链接多个哦
                var socketContext = await client.ConnectAsync(new IPEndPoint(IPUtility.GetLocalIntranetIP(), 5566));

                //发送消息
                var initCmd = new Peach.Messaging.CommandLineMessage("init");
                await socketContext.SendAsync(initCmd);
                //发送消息2
                var echoCmd = new Peach.Messaging.CommandLineMessage("echo", "hello");
                await socketContext.SendAsync(echoCmd);

               
                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
                //关闭链接
                await client.ShutdownGracefullyAsync(2000, 2000);

            }).Wait();
         
        }
    }
}
