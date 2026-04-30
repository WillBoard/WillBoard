using System;
using Mediator;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationUpdate
{
    public class NavigationUpdateQuery : IRequest<Result<NavigationUpdateViewModel, InternalError>>
    {
        public Guid NavigationId { get; set; }
    }
}
