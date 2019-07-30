// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System.Collections.Generic;
    using DotNetty.Codecs.Mqtt.Packets;

    /// <summary>
    /// 负责每个客户端的订阅管理
    /// </summary>
    public class MqttClientSubscriptionsManager
    {
        private readonly Dictionary<string, QualityOfService> _subscriptions = new Dictionary<string, QualityOfService>();
        
        private readonly string _clientId;

        public MqttClientSubscriptionsManager(string clientId)
        {
            this._clientId = clientId;
        } 

        //TODO:处理订阅的逻辑
        
        
        //TODO:处理取消订阅的逻辑
        
        
    }
}