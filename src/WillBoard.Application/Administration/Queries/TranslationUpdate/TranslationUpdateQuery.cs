using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.TranslationUpdate
{
    public class TranslationUpdateQuery : IRequest<Result<TranslationUpdateViewModel, InternalError>>
    {
        public string Language { get; set; }
        public string Key { get; set; }
    }
}