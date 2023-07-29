using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Configurations
{
    public class ConfigurationsQuery : IRequest<Result<ConfigurationsViewModel, InternalError>>
    {
    }
}