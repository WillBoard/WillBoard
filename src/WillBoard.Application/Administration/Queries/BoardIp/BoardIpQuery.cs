using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardIp
{
    public class BoardIpQuery : IRequest<Result<BoardIpViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public IpVersion IpVersion { get; set; }
        public string IpNumber { get; set; }
    }
}