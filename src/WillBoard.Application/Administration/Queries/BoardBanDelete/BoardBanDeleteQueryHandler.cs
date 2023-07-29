using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanDelete
{
    public class BoardBanDeleteQueryHandler : IRequestHandler<BoardBanDeleteQuery, Result<BoardBanDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;

        public BoardBanDeleteQueryHandler(AccountManager accountManager, IBoardCache boardCache, IBanCache banCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _banCache = banCache;
        }

        public async Task<Result<BoardBanDeleteViewModel, InternalError>> Handle(BoardBanDeleteQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardBanDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanDelete))
            {
                return Result<BoardBanDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetBoardAsync(request.BoardId, request.BanId);

            if (ban == null || ban.BoardId != request.BoardId)
            {
                return Result<BoardBanDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardBanDeleteViewModel()
            {
                Ban = ban
            };

            return Result<BoardBanDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}