using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class BanAppeal
    {
        public Guid BanAppealId { get; set; }
        public DateTime Creation { get; set; }
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }
        public string Message { get; set; }
        public Guid BanId { get; set; }

        public virtual Ban Ban { get; set; }

        public BanAppeal()
        {
        }
    }
}