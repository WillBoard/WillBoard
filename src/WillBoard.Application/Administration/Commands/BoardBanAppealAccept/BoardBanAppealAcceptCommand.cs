using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardBanAppealAccept
{
    public class BoardBanAppealAcceptCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid BanAppealId { get; set; }
    }
}