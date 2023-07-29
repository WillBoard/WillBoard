using MediatR;
using System;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BanCreate
{
    public class BanCreateCommand : IRequest<Status<InternalError>>
    {
        public IpVersion IpVersion { get; set; }
        public string IpNumber { get; set; }
        public byte Cidr { get; set; }
        public string ExclusionIpNumberCollection { get; set; }
        public DateTime? Expiration { get; set; }
        public bool Appeal { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}