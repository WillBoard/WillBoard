using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.ConfigurationUpdate
{
    public class ConfigurationUpdateCommand : IRequest<Status<InternalError>>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public ConfigurationType Type { get; set; }
    }
}