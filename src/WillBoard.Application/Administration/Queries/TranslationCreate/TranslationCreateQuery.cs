using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.TranslationCreate
{
    public class TranslationCreateQuery : IRequest<Result<TranslationCreateViewModel, InternalError>>
    {
    }
}