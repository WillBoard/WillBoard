using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.TranslationCreate
{
    public class TranslationCreateCommand : IRequest<Status<InternalError>>
    {
        public string Language { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}