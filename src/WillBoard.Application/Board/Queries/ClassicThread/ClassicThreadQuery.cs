using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.ClassicThread
{
    public class ClassicThreadQuery : IRequest<Result<ClassicThreadViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int ThreadId { get; set; }
        public int? Last { get; set; }
    }
}