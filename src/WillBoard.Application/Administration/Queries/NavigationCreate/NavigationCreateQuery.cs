using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationCreate
{
    public class NavigationCreateQuery : IRequest<Result<NavigationCreateViewModel, InternalError>>
    {
    }
}