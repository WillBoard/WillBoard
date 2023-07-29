using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountAuthentications
{
    public class AccountAuthenticationsQuery : IRequest<Result<AccountAuthenticationsViewModel, InternalError>>
    {
        public Guid AccountId { get; set; }
        public int Page { get; set; }
    }
}