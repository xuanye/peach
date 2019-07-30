// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System;
    using System.Collections.Generic;
    using DotNetty.Codecs.Mqtt.Packets;
    using Microsoft.Extensions.DependencyInjection;
 

    public class PacketProcessorManager
    {
        readonly IServiceProvider serviceProvider;
        readonly Dictionary<PacketType,IPacketProcessor> processors = new Dictionary<PacketType, IPacketProcessor>();
        public PacketProcessorManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            Init();
        }

        void Init()
        {
           var pList =   this.serviceProvider.GetServices<IPacketProcessor>();
           foreach (var processor in pList)
           {
               if (!processors.ContainsKey(processor.PacketType))
               {
                   processors.Add(processor.PacketType, processor);
               }
           }
          
        }

        public IPacketProcessor GetProcessor(PacketType packetType)
        {
            if (processors.ContainsKey(packetType))
            {
                return processors[packetType];
            }

            return null;
        }
    }
}