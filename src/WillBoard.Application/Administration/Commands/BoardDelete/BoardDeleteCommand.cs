using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardDelete
{
    public class BoardDeleteCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
    }
}