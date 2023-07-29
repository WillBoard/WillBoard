using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardBanDelete
{
    public class BoardBanDeleteCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid BanId { get; set; }
    }
}