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

namespace WillBoard.Application.Administration.Queries.BoardViewCatalog
{
    public class BoardViewCatalogQueryHandler : IRequestHandler<BoardViewCatalogQuery, Result<BoardViewCatalogViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;

        public BoardViewCatalogQueryHandler(AccountManager accountManager, BoardManager boardManager, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardManager = boardManager;
            _postCache = postCache;
        }

        public async Task<Result<BoardViewCatalogViewModel, InternalError>> Handle(BoardViewCatalogQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<BoardViewCatalogViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (!board.CatalogAvailability)
            {
                return Result<BoardViewCatalogViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            IEnumerable<Post> postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var threadCollection = postCollection.Where(p => p.ThreadId == null).OrderByDescending(p => p.Pin).ThenByDescending(p => p.Bump);

            var result = new BoardViewCatalogViewModel()
            {
                BoardViewType = BoardViewType.Catalog,
                Title = $"/{board.BoardId}/ - {board.Name}",
                ThreadCollection = threadCollection
            };

            return Result<BoardViewCatalogViewModel, InternalError>.ValueResult(result);
        }
    }
}