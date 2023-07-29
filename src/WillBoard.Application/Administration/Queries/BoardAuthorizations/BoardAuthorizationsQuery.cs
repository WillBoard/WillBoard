using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizations
{
    public class BoardAuthorizationsQuery : IRequest<Result<BoardAuthorizationsViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int Page { get; set; }
    }
}