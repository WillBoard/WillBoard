using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardUpdate
{
    public class BoardUpdateQuery : IRequest<Result<BoardUpdateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}