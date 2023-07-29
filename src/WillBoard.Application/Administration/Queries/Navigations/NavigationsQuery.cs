using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Navigations
{
    public class NavigationsQuery : IRequest<Result<NavigationsViewModel, InternalError>>
    {
    }
}