using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardViewSearch
{
    public class BoardViewSearchQuery : IRequest<Result<BoardViewSearchViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int? PostId { get; set; }
        public int? ThreadId { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
        public string Type { get; set; }
        public string Order { get; set; }
    }
}