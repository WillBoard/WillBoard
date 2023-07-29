using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardViewClassicThread
{
    public class BoardViewClassicThreadQuery : IRequest<Result<BoardViewClassicThreadViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int ThreadId { get; set; }
        public int Last { get; set; }
    }
}