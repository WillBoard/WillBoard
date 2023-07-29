using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Catalog
{
    public class CatalogQueryHandler : IRequestHandler<CatalogQuery, Result<CatalogViewModel, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;

        public CatalogQueryHandler(BoardManager boardManager, IPostCache postCache)
        {
            _boardManager = boardManager;
            _postCache = postCache;
        }

        public async Task<Result<CatalogViewModel, InternalError>> Handle(CatalogQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!board.CatalogAvailability)
            {
                return Result<CatalogViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            IEnumerable<Post> postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var threadCollection = postCollection.Where(p => p.ThreadId == null).OrderByDescending(p => p.Pin).ThenByDescending(p => p.Bump);

            var result = new CatalogViewModel()
            {
                BoardViewType = BoardViewType.Catalog,
                Title = $"/{board.BoardId}/ - {board.Name}",
                ThreadCollection = threadCollection
            };

            return Result<CatalogViewModel, InternalError>.ValueResult(result);
        }
    }
}