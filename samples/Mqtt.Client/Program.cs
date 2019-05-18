using System;

namespace Mqtt.Client
{
    using System.Net;
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Peach.Infrastructure;
    using Peach.Mqtt;
    using Peach.Tcp;

    class Program
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
            TcpClient<MqttMessage> client = new TcpClient<MqttMessage>(new MqttChannelHandlerPipeline(new MqttOptions()));
            client.OnReceived += Client_OnReceived;
            client.OnConnected += Client_OnConnected;

            Task.Run(async () =>
            {
                //连接服务器，可以链接多个哦
                var socketContext = await client.ConnectAsync(new IPEndPoint(IPUtility.GetLocalIntranetIP(), 5566));

                //发送消息

                var connPack = new ConnectPacket {  ClientId = Guid.NewGuid().ToString("N")};
                await socketContext.SendAsync(new MqttMessage { Packet = connPack});

              
                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
                //关闭链接
                await client.ShutdownGracefullyAsync(2000, 2000);

            }).Wait();

        }

        static void Client_OnConnected(object sender, Peach.EventArgs.ConnectedEventArgs<MqttMessage> e)
        {
            Console.WriteLine("server is connected");
        }

        static void Client_OnReceived(object sender, Peach.EventArgs.MessageReceivedEventArgs<MqttMessage> e)
        {
            string content = $"PacketType = {e.Message.Packet.PacketType}";
            Console.WriteLine("receive message {0} from {1}", content, e.Context.RemoteEndPoint);
        }
    }
}