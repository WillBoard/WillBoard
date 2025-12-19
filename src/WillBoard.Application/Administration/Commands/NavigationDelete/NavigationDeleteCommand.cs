using System;
using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.NavigationDelete
{
    public class NavigationDeleteCommand : IRequest<Status<InternalError>>
    {
        public Guid NavigationId { get; set; }
    }
}
