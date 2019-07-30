// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    public class MqttClientSession
    {
        readonly MqttClientSubscriptionsManager _subscriptionManager;


        public MqttClientSession(string clientId)
        {
            this._subscriptionManager = new MqttClientSubscriptionsManager(clientId);
            this.ClientId = clientId;
        }
        
        
             
        public string ClientId { get;  }
    
    }
}