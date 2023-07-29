using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardDelete
{
    public class BoardDeleteQuery : IRequest<Result<BoardDeleteViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}