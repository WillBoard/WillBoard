using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BanDelete
{
    public class BanDeleteCommand : IRequest<Status<InternalError>>
    {
        public Guid BanId { get; set; }
    }
}