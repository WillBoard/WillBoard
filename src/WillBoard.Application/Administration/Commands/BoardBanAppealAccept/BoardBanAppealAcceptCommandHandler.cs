using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardBanAppealAccept
{
    public class BoardBanAppealAcceptCommandHandler : IRequestHandler<BoardBanAppealAcceptCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;

        public BoardBanAppealAcceptCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBoardCache boardCache, IBanCache banCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _boardCache = boardCache;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BoardBanAppealAcceptCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanAppealAccept))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banAppeal = await _banAppealCache.GetBoardAsync(board.BoardId, request.BanAppealId);
            if (banAppeal == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _banRepository.DeleteAsync(banAppeal.BanId);

            await _banCache.RemoveBoardAsync(board.BoardId, banAppeal.BanId);
            await _banCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
            await _banCache.RemoveBoardCountAsync(board.BoardId);

            await _banAppealCache.RemoveBoardAsync(board.BoardId, banAppeal.BanAppealId);
            await _banAppealCache.RemoveBoardBanCollectionAsync(board.BoardId, banAppeal.BanId);
            await _banAppealCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banAppealCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}