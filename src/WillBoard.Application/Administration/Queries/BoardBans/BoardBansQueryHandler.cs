using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBans
{
    public class BoardBansQueryHandler : IRequestHandler<BoardBansQuery, Result<BoardBansViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;

        public BoardBansQueryHandler(AccountManager accountManager, IBoardCache boardCache, IBanCache banCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _banCache = banCache;
        }

        public async Task<Result<BoardBansViewModel, InternalError>> Handle(BoardBansQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardBansViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanRead))
            {
                return Result<BoardBansViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banCount = await _banCache.GetBoardCountAsync(board.BoardId);
            var pageMax = (banCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardBansViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var banCollection = await _banCache.GetBoardCollectionAsync(board.BoardId, (request.Page - 1) * 100, 100);

            var result = new BoardBansViewModel()
            {
                Board = board,
                BanCollection = banCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardBansViewModel, InternalError>.ValueResult(result);
        }
    }
}