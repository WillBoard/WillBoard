using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadReplyLock
{
    public class BoardThreadReplyLockQuery : IRequest<Result<BoardThreadReplyLockViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public bool ReplyLock { get; set; }
    }
}