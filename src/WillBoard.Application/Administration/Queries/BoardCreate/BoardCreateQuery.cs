using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardCreate
{
    public class BoardCreateQuery : IRequest<Result<BoardCreateViewModel, InternalError>>
    {
    }
}