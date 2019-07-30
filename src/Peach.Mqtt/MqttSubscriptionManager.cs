// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public class MqttSubscriptionManager
    {
        //客户端订阅列表
        static  readonly  ConcurrentDictionary<string,List<MqttSubscription>> 
            CLIENT_DIC = new ConcurrentDictionary<string,List<MqttSubscription>>();
        
        //主体关联客户端列表
        static readonly ConcurrentDictionary<string,List<string>>  TOPIC_DIC = new ConcurrentDictionary<string, List<string>>();
        
        public MqttSubscriptionManager()
        {
            
        }


        public Task<IMqttResult> Subscribe(MqttClientSession clientSession,SubscribePacket subscribePacket)
        {

            if (!CLIENT_DIC.TryGetValue(clientSession.ClientId, out List<MqttSubscription> subList))
            {
                subList = new List<MqttSubscription>();

                CLIENT_DIC.TryAdd(clientSession.ClientId, subList);
            }
            
            foreach (SubscriptionRequest subReq in subscribePacket.Requests)
            {
                if (!subList.Exists(x => x.Topic.Equals(subReq.TopicFilter, StringComparison.OrdinalIgnoreCase)))
                {
                    subList.Add(new MqttSubscription {
                        ClientSession = clientSession,
                        Topic = subReq.TopicFilter,
                        Qos =  subReq.QualityOfService
                    });
                }

                if (!TOPIC_DIC.TryGetValue(subReq.TopicFilter, out List<string> clientIds))
                {
                    clientIds = new List<string>();
                    TOPIC_DIC.TryAdd(subReq.TopicFilter, clientIds);
                }
                clientIds.Add(clientSession.ClientId);
                
            }
            
            return Task.FromResult<IMqttResult>(SubscribeResult.SUCCESS);
            
        }

        internal Task<List<string>> GetSubscribeClientIds(string topicName)
        {
            throw new NotImplementedException();
        }

        public Task<IMqttResult> UnSubscribe(MqttClientSession clientSession, UnsubscribePacket unSubscribePacket)
        {
            if (CLIENT_DIC.TryGetValue(clientSession.ClientId, out List<MqttSubscription> subList))
            {
                foreach (string topicFilter in unSubscribePacket.TopicFilters)
                {
                    subList.RemoveAll(x => x.Topic == topicFilter);
                    
                    if (TOPIC_DIC.TryGetValue(topicFilter, out List<string> clientIds))
                    {
                        clientIds.Remove(clientSession.ClientId);
                    }
                }
            }
            
            return Task.FromResult<IMqttResult>(SubscribeResult.SUCCESS);
        }
        
        
        
    }
}