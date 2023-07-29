using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountInvitationReject
{
    public class AccountInvitationRejectCommand : IRequest<Status<InternalError>>
    {
        public Guid AccountId { get; set; }
        public Guid InvitationId { get; set; }
    }
}