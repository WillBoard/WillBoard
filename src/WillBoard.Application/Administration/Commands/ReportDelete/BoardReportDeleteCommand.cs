using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.ReportDelete
{
    public class ReportDeleteCommand : IRequest<Status<InternalError>>
    {
        public Guid ReportId { get; set; }
    }
}