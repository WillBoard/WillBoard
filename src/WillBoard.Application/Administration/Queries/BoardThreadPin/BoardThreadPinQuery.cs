using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadPin
{
    public class BoardThreadPinQuery : IRequest<Result<BoardThreadPinViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public bool Pin { get; set; }
    }
}