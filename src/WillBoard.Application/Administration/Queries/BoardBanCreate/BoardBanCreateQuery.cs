using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanCreate
{
    public class BoardBanCreateQuery : IRequest<Result<BoardBanCreateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public IpVersion IPVersion { get; set; }
        public string IPNumber { get; set; }
    }
}