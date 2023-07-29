using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanUpdate
{
    public class BanUpdateQuery : IRequest<Result<BanUpdateViewModel, InternalError>>
    {
        public Guid BanId { get; set; }
    }
}