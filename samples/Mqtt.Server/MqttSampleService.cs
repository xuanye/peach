// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mqtt.Server
{
    using System;
    using Microsoft.Extensions.Logging;
    using Peach;
    using Peach.Mqtt;

    public class MqttSampleService:AbsMqttSocketService
    {
        readonly ILogger<MqttSampleService> logger;


        public MqttSampleService(PacketProcessorManager processorManager,MqttClientSessionManager sessionManager,ILoggerFactory loggerFactory)
            :base(processorManager,sessionManager,loggerFactory)
        {
            logger = loggerFactory.CreateLogger<MqttSampleService>();
        }
        public override void OnConnected(ISocketContext<MqttMessage> context)
        {
            logger.LogInformation("client connected from {0}", context.RemoteEndPoint);  
            base.OnConnected(context);
        }

        public override void OnDisconnected(ISocketContext<MqttMessage> context)
        {
            logger.LogInformation("client disconnected from {0}", context.RemoteEndPoint);
            base.OnDisconnected(context);
        }

        public override void OnException(ISocketContext<MqttMessage> context, Exception ex)
        {
            logger.LogError(ex, "client from {0},  occ error {1}", context.RemoteEndPoint, ex.Message);
            base.OnException(context, ex);
        }
    }
}