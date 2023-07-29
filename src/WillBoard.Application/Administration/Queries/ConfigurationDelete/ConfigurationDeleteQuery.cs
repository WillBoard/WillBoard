using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationDelete
{
    public class ConfigurationDeleteQuery : IRequest<Result<ConfigurationDeleteViewModel, InternalError>>
    {
        public string Key { get; set; }
    }
}