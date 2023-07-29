using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BanAppealReject
{
    public class BanAppealRejectCommand : IRequest<Status<InternalError>>
    {
        public Guid BanAppealId { get; set; }
    }
}