using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanAppeals
{
    public class BoardBanAppealsQueryHandler : IRequestHandler<BoardBanAppealsQuery, Result<BoardBanAppealsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IBanAppealCache _banAppealCache;

        public BoardBanAppealsQueryHandler(AccountManager accountManager, IBoardCache boardCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Result<BoardBanAppealsViewModel, InternalError>> Handle(BoardBanAppealsQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardBanAppealsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanAppealRead))
            {
                return Result<BoardBanAppealsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banAppealLocalCount = await _banAppealCache.GetBoardCountAsync(board.BoardId);
            var pageMax = (banAppealLocalCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardBanAppealsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var banAppealCollection = await _banAppealCache.GetBoardCollectionAsync(board.BoardId, (request.Page - 1) * 100, 100);

            var result = new BoardBanAppealsViewModel()
            {
                Board = board,
                BanAppealCollection = banAppealCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardBanAppealsViewModel, InternalError>.ValueResult(result);
        }
    }
}