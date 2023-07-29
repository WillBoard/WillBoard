using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountPasswordChange
{
    public class AccountPasswordChangeQuery : IRequest<Result<AccountPasswordChangeViewModel, InternalError>>
    {
        public Guid AccountId { get; set; }
    }
}