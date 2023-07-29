using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.TranslationDelete
{
    public class TranslationDeleteQuery : IRequest<Result<TranslationDeleteViewModel, InternalError>>
    {
        public string Language { get; set; }
        public string Key { get; set; }
    }
}