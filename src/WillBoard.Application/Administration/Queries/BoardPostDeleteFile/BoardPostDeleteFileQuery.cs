using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardPostDeleteFile
{
    public class BoardPostDeleteFileQuery : IRequest<Result<BoardPostDeleteFileViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}