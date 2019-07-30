// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    public interface IMqttResult
    {
        int Code { get; }
        
        string Message { get; }
    }

    public class MqttResult:IMqttResult
    {
        public static readonly  SubscribeResult SUCCESS = new SubscribeResult();
        
        public int Code { get; set; }

        public string Message { get; set; }
    }
    
    public class SubscribeResult:IMqttResult
    {
        public static readonly  SubscribeResult SUCCESS = new SubscribeResult();
        public int Code { get; }

        public string Message { get; }
    }
}