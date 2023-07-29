using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Catalog
{
    public class CatalogQuery : IRequest<Result<CatalogViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}