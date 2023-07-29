using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Classic
{
    public class ClassicQuery : IRequest<Result<ClassicViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int Page { get; set; }
    }
}