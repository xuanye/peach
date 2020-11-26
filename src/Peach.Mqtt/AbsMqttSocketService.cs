// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.Logging;

    public abstract class AbsMqttSocketService:AbsSocketService<MqttMessage>
    {
        readonly PacketProcessorManager _processorManager;
        readonly MqttClientSessionManager _sessionManager;
        readonly ILogger<AbsMqttSocketService> _logger;

        public AbsMqttSocketService(PacketProcessorManager processorManager,MqttClientSessionManager sessionManager,ILoggerFactory loggerFactory)
        {
            this._processorManager = processorManager;
            this._sessionManager = sessionManager;
            this._logger = loggerFactory.CreateLogger<AbsMqttSocketService>();
        }
        
        
        public override void OnReceive(ISocketContext<MqttMessage> context, MqttMessage msg)
        {
            this._logger.LogTrace("receive Packet from {0}",context.RemoteEndPoint);
         
            if (msg?.Packet == null)
            {
                this._logger.LogWarning("receive receive message from {0}, but packet is null",context.RemoteEndPoint);
                return;
            }
        
            Task.Run(
                async () =>
                { 
                    this._logger.LogDebug("receive Packet from {0}, type ={1}",context.RemoteEndPoint,msg.Packet.PacketType);

                    MqttClientSession clientSession = this._sessionManager.GetClientSession(context, msg.Packet);
                    
                    IPacketProcessor processor =  this._processorManager.GetProcessor(msg.Packet.PacketType);
                    if (processor != null)
                    {
                       MqttMessage rsp = await processor.ProcessAsync(clientSession,msg.Packet);
                       if ( rsp != null )
                       {
                           if (rsp.Packet != null)
                           {
                               await context.SendAsync(rsp);
                           }

                           if (rsp.Code != 0) //主动断开
                           {
                               await ShutdownChannel(context, msg.Packet);
                           }
                           
                       }
                    }
                    else
                    {
                        this._logger.LogWarning("PacketType:{0} has no processor",msg.Packet.PacketType);
                    }
                    
                }).ConfigureAwait(false);
           
            /*
            switch (msg.Packet.PacketType)
            {
                case  PacketType.CONNECT:
                    Console.WriteLine("receive connect pack client_id = {0}",((ConnectPacket)msg.Packet).ClientId);
                    var ack = new ConnAckPacket { ReturnCode = ConnectReturnCode.Accepted, SessionPresent = true};
                    context.SendAsync(new MqttMessage { Packet =  ack});
                    break;
                case PacketType.PINGREQ:
                    context.SendAsync(new MqttMessage { Packet =   PingRespPacket.Instance});
                    break;
                default:
                    Console.WriteLine("receive Packet from {0}, type ={1}",context.RemoteEndPoint,msg.Packet.PacketType);
                    break;
                
            }*/
           
        }

        Task ShutdownChannel(ISocketContext<MqttMessage> context, Packet packet)
        {
            //TODO:断开链接前 需要清理的数据 和一些事件处理
            return context.Channel.CloseAsync();
        }
    }
}