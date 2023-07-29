using System;
using System.Numerics;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Verification
    {
        public Guid VerificationId { get; set; }
        public string BoardId { get; set; }
        public IpVersion IpVersion { get; set; }
        public BigInteger IpNumber { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Expiration { get; set; }

        public Verification()
        {
        }
    }
}