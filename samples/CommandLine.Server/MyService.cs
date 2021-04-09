using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Peach;
using Peach.Messaging;
using Microsoft.Extensions.Logging;

namespace CommandLine.Server
{
    public class MyService : Peach.AbsSocketService<Peach.Messaging.CommandLineMessage>
    {
        private readonly ILogger<MyService> _logger;


        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }
        public override void OnConnected(ISocketContext<CommandLineMessage> context)
        {
            _logger.LogInformation("client connected from {0}", context.RemoteEndPoint);
            base.OnConnected(context);
        }

        public override void OnDisconnected(ISocketContext<CommandLineMessage> context)
        {
            _logger.LogInformation("client disconnected from {0}", context.RemoteEndPoint);
            base.OnDisconnected(context);
        }

        public override void OnException(ISocketContext<CommandLineMessage> context, Exception ex)
        {
            _logger.LogError(ex, "client from {0},  occ error {1}", context.RemoteEndPoint, ex.Message);
            base.OnException(context, ex);
        }

        public override void OnReceive(ISocketContext<CommandLineMessage> context, CommandLineMessage msg)
        {
            _logger.LogInformation("receive msg from {0},{1}", context.RemoteEndPoint, msg.Command);
            string replyMessage = string.Empty;
            string replyCmd = string.Empty;
            switch (msg.Command)
            {
                case "echo":
                    replyMessage = msg.Parameters[0];
                    replyCmd = "echo";
                    break;
                case "init":
                    replyMessage = "ok";
                    replyCmd = "init_reply";

                    break;
                case "idle":
                    replyMessage = "ok";
                    replyCmd = "idle_reply";
                    break;
                default:
                    replyMessage = "error unknow command";
                    break;
            }


            Task.Run(async () =>
            {
                await context.SendAsync(new CommandLineMessage(replyCmd, replyMessage));
            });
        }

    }
}
