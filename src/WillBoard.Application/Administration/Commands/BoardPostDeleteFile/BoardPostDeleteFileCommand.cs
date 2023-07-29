using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardPostDeleteFile
{
    public class BoardPostDeleteFileCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}