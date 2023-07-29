using System;
using System.Numerics;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Ban
    {
        public Guid BanId { get; set; }

        public string BoardId { get; set; }

        public DateTime Creation { get; set; }
        public DateTime? Expiration { get; set; }
        public bool Appeal { get; set; }

        public IpVersion IpVersion { get; set; }
        public BigInteger IpNumberFrom { get; set; }
        public BigInteger IpNumberTo { get; set; }

        public BigInteger[] ExclusionIpNumberCollection { get; set; }

        public string Reason { get; set; }
        public string Note { get; set; }

        public Ban()
        {
        }
    }
}