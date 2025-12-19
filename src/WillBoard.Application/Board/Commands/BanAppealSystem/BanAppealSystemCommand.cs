using System;
using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.BanAppealSystem
{
    public class BanAppealSystemCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid BanId { get; set; }
        public string Message { get; set; }
    }
}
