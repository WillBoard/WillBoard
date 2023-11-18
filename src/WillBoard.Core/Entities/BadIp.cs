using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class BadIp
    {
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumberFrom { get; set; }
        public UInt128 IpNumberTo { get; set; }

        public BadIp()
        {
        }
    }
}