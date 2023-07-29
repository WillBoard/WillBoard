using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardInvitations
{
    public class BoardInvitationsQuery : IRequest<Result<BoardInvitationsViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int Page { get; set; }
    }
}