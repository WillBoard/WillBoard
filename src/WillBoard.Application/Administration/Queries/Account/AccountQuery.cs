using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Account
{
    public class AccountQuery : IRequest<Result<AccountViewModel, InternalError>>
    {
        public Guid AccountId { get; set; }
    }
}