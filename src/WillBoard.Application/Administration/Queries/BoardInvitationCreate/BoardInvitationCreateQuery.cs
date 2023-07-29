using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardInvitationCreate
{
    public class BoardInvitationCreateQuery : IRequest<Result<BoardInvitationCreateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}