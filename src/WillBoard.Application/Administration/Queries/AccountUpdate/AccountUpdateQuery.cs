using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountUpdate
{
    public class AccountUpdateQuery : IRequest<Result<AccountUpdateViewModel, InternalError>>
    {
        public Guid AccountId { get; set; }
    }
}