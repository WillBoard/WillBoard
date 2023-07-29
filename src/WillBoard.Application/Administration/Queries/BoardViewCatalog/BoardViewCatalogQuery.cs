using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardViewCatalog
{
    public class BoardViewCatalogQuery : IRequest<Result<BoardViewCatalogViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}