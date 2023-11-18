using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Authentication
    {
        public Guid AuthenticationId { get; set; }
        public Guid AccountId { get; set; }
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Expiration { get; set; }
        public string Name { get; set; }

        public Authentication()
        {
        }

        public Authentication(Authentication authentication)
        {
            AuthenticationId = authentication.AuthenticationId;
            AccountId = authentication.AccountId;
            IpVersion = authentication.IpVersion;
            IpNumber = authentication.IpNumber;
            Creation = authentication.Creation;
            Expiration = authentication.Expiration;
            Name = authentication.Name;
        }
    }
}