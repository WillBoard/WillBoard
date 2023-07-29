using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardViewSearch
{
    public class BoardViewSearchQueryHandler : IRequestHandler<BoardViewSearchQuery, Result<BoardViewSearchViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;

        public BoardViewSearchQueryHandler(AccountManager accountManager, BoardManager boardManager, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardManager = boardManager;
            _postCache = postCache;
        }

        public async Task<Result<BoardViewSearchViewModel, InternalError>> Handle(BoardViewSearchQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<BoardViewSearchViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (!board.SearchAvailability)
            {
                return Result<BoardViewSearchViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var postCollection = await _postCache.GetAdaptedSearchAsync(board, request.PostId, request.ThreadId, request.Message, request.File, request.Type, request.Order);

            var result = new BoardViewSearchViewModel()
            {
                BoardViewType = BoardViewType.Search,
                Title = $"/{board.BoardId}/ - {board.Name}",
                SearchCollection = postCollection,
                PostId = request.PostId,
                ThreadId = request.ThreadId,
                Message = request.Message,
                File = request.File,
                Type = request.Type,
                Order = request.Order,
            };

            return Result<BoardViewSearchViewModel, InternalError>.ValueResult(result);
        }
    }
}