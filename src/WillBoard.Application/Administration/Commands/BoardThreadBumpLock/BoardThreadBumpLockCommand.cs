using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardThreadBumpLock
{
    public class BoardThreadBumpLockCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public bool BumpLock { get; set; }
    }
}