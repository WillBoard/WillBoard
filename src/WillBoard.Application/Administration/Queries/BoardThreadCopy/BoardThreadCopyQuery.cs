using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadCopy
{
    public class BoardThreadCopyQuery : IRequest<Result<BoardThreadCopyViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}