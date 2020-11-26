// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System.Collections.Concurrent;
    using DotNetty.Codecs.Mqtt.Packets;

    public class MqttClientSessionManager
    {
        //链接信息
        readonly ConcurrentDictionary<string, MqttClientConnection> _connections = new ConcurrentDictionary<string, MqttClientConnection>();
        
        //会话信息
        readonly ConcurrentDictionary<string, MqttClientSession> _sessions = new ConcurrentDictionary<string, MqttClientSession>();
        
        public MqttClientSessionManager()
        {
            
        }
        
        
        public MqttClientSession GetClientSession(ISocketContext<MqttMessage> context, Packet msgPacket)
        {
            MqttClientSession session = null;
            if (msgPacket.PacketType == PacketType.CONNECT)
            {
                if (msgPacket is ConnectPacket connectPacket)
                    session = new MqttClientSession(connectPacket.ClientId);

                if (session == null)
                {
                    return null;
                }
                
                _sessions.TryAdd(session.ClientId, session);
                return session;
            }

            _sessions.TryGetValue(context.Id, out session);
            return session;
        }
    }
}