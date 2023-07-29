using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadBumpLock
{
    public class BoardThreadBumpLockQuery : IRequest<Result<BoardThreadBumpLockViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public bool BumpLock { get; set; }
    }
}