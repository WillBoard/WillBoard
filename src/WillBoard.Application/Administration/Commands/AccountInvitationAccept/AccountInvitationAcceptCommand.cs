using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountInvitationAccept
{
    public class AccountInvitationAcceptCommand : IRequest<Status<InternalError>>
    {
        public Guid AccountId { get; set; }
        public Guid InvitationId { get; set; }
    }
}