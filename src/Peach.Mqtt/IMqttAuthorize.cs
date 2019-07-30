// Copyright (c) Xuanye. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Peach.Mqtt
{
    using System.Threading.Tasks;
    using DotNetty.Codecs.Mqtt.Packets;

    public interface IMqttAuthorize
    {
        Task<IMqttResult> Validate(ConnectPacket packet);
    }

    public class NoopMqttAuthorize : IMqttAuthorize
    {
        public Task<IMqttResult> Validate(ConnectPacket packet)
        {  
            var result = new MqttResult();
            if (packet == null)
            {
                result.Code = MqttErrorCodes.AUTHORIZE_FAILED_CODE;
                result.Message = "data is null";
                return Task.FromResult<IMqttResult>(result);
            }
          
            if (string.IsNullOrEmpty(packet.ClientId))
            {
                result.Code = MqttErrorCodes.AUTHORIZE_FAILED_CODE;
                result.Message = "clientId is required";
            }
            return Task.FromResult<IMqttResult>(result);
        }
    }
}