using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanDelete
{
    public class BanDeleteQuery : IRequest<Result<BanDeleteViewModel, InternalError>>
    {
        public Guid BanId { get; set; }
    }
}