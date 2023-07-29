using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Translations
{
    public class TranslationsQuery : IRequest<Result<TranslationsViewModel, InternalError>>
    {
    }
}