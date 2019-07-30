using System;
using System.Collections.Generic;
using System.Text;

namespace Peach.Config
{
    public class TcpHostOption
    {
        public AddressBindType BindType { get; set; } = AddressBindType.InternalAddress;

        public int Port { get; set; } = 5566;

        public int SoBacklog { get; set; } = 128;


        public int QuietPeriod { get; set; } = 3;

        public int ShutdownTimeout { get; set; } = 3;

        /// <summary>
        /// 无效的配置，使用Libuv有问题，暂时没找到原因
        /// </summary>
        public bool UseLibuv { get; set; } = false;

        public string SpecialAddress { get; set; } = "127.0.0.1";

        public string StartupWords { get; set; } = "TcpServerHost bind at {0} \r\n";

        public string AppName { get; set; } = "PeachApp";

        public string Certificate { get; set; }

        public string CertificatePassword { get; set; }
    }

    public enum AddressBindType
    {
        Any, //0.0.0.0 Address.Any
        Loopback, // localhost
        InternalAddress, //本机内网地址
        SpecialAddress //自定义地址
    }
}
