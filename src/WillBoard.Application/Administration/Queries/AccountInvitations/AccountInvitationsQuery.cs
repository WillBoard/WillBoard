using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountInvitations
{
    public class AccountInvitationsQuery : IRequest<Result<AccountInvitationsViewModel, InternalError>>
    {
        public Guid AccountId { get; set; }
        public int Page { get; set; }
    }
}