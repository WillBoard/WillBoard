using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardPostUpdate
{
    public class BoardPostUpdateQuery : IRequest<Result<BoardPostUpdateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}