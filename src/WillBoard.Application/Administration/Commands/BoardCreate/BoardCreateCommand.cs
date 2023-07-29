using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardCreate
{
    public class BoardCreateCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
    }
}