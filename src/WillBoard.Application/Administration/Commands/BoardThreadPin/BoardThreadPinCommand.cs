using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardThreadPin
{
    public class BoardThreadPinCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public bool Pin { get; set; }
    }
}