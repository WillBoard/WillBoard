using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationCreate
{
    public class ConfigurationCreateQuery : IRequest<Result<ConfigurationCreateViewModel, InternalError>>
    {
    }
}