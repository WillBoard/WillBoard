using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.ConfigurationDelete
{
    public class ConfigurationDeleteCommand : IRequest<Status<InternalError>>
    {
        public string Key { get; set; }
    }
}