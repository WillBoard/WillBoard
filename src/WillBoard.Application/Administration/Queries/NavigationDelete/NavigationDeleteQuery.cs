using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationDelete
{
    public class NavigationDeleteQuery : IRequest<Result<NavigationDeleteViewModel, InternalError>>
    {
        public Guid NavigationId { get; set; }
    }
}