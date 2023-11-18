using System;
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
        public UInt128 IpNumberFrom { get; set; }
        public UInt128 IpNumberTo { get; set; }

        public UInt128[] ExclusionIpNumberCollection { get; set; }

        public string Reason { get; set; }
        public string Note { get; set; }

        public Ban()
        {
        }
    }
}