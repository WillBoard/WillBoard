using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardInvitationDelete
{
    public class BoardInvitationDeleteCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid InvitationId { get; set; }
    }
}