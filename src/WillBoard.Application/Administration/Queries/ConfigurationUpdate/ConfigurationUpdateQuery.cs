using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationUpdate
{
    public class ConfigurationUpdateQuery : IRequest<Result<ConfigurationUpdateViewModel, InternalError>>
    {
        public string Key { get; set; }
    }
}