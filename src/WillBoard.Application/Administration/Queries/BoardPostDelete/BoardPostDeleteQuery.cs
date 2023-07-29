using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardPostDelete
{
    public class BoardPostDeleteQuery : IRequest<Result<BoardPostDeleteViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}