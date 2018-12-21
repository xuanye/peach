using System;
using System.Net;
using System.Threading.Tasks;

namespace CommandLine.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MyCommandClient client = new MyCommandClient(new Hey.Protocol.CommandLineProtocol());

            Task.Run(async () =>
            {
                var socketContext = await client.ConnectAsync(new IPEndPoint(Hey.IPUtility.GetLocalIntranetIP(), 5566));

                var initCmd = new Hey.Messaging.CommandLineMessage("init");
                await socketContext.SendAsync(initCmd);

                var echoCmd = new Hey.Messaging.CommandLineMessage("echo", "hello");
                await socketContext.SendAsync(echoCmd);

               
                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
                await client.ShutdownGracefullyAsync(3000, 3000);

            }).Wait();
         
        }
    }
}
