using MediatR;
using System;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BanUpdate
{
    public class BanUpdateCommand : IRequest<Status<InternalError>>
    {
        public Guid BanId { get; set; }
        public IpVersion IpVersion { get; set; }
        public string IpNumberFrom { get; set; }
        public string IpNumberTo { get; set; }
        public string ExclusionIpNumberCollection { get; set; }
        public DateTime? Expiration { get; set; }
        public bool Appeal { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}