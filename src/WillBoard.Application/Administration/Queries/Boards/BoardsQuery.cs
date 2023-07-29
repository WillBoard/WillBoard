using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Boards
{
    public class BoardsQuery : IRequest<Result<BoardsViewModel, InternalError>>
    {
        public int Page { get; set; }
    }
}