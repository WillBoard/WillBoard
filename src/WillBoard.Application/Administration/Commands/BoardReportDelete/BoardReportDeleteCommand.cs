using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardReportDelete
{
    public class BoardReportDeleteCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid ReportId { get; set; }
    }
}