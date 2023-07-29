using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.TranslationDelete
{
    public class TranslationDeleteCommand : IRequest<Status<InternalError>>
    {
        public string Language { get; set; }
        public string Key { get; set; }
    }
}