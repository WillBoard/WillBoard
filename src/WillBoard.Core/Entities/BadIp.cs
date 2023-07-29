using System.Numerics;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class BadIp
    {
        public IpVersion IpVersion { get; set; }
        public BigInteger IpNumberFrom { get; set; }
        public BigInteger IpNumberTo { get; set; }

        public BadIp()
        {
        }
    }
}