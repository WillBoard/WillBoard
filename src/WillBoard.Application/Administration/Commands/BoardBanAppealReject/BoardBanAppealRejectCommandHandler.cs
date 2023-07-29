using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardBanAppealReject
{
    public class BoardBanAppealRejectCommandHandler : IRequestHandler<BoardBanAppealRejectCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanAppealRepository _banAppealRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanAppealCache _banAppealCache;

        public BoardBanAppealRejectCommandHandler(AccountManager accountManager, IBanAppealRepository banAppealRepository, IBoardCache boardCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banAppealRepository = banAppealRepository;
            _boardCache = boardCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BoardBanAppealRejectCommand request, CancellationToken cancellationToken)
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

            await _banAppealRepository.DeleteAsync(banAppeal.BanAppealId);

            await _banAppealCache.RemoveBoardAsync(board.BoardId, banAppeal.BanAppealId);
            await _banAppealCache.RemoveBoardBanCollectionAsync(board.BoardId, banAppeal.BanId);
            await _banAppealCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banAppealCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}