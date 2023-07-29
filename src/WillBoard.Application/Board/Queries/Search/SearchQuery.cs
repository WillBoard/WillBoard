using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Search
{
    public class SearchQuery : IRequest<Result<SearchViewModel, InternalError>>
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