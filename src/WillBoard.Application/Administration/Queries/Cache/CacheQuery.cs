using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Cache
{
    public class CacheQuery : IRequest<Result<CacheViewModel, InternalError>>
    {
    }
}