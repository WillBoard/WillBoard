using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardBanDelete
{
    public class BoardBanDeleteCommandHandler : IRequestHandler<BoardBanDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;

        public BoardBanDeleteCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBoardCache boardCache, IBanCache banCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _boardCache = boardCache;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BoardBanDeleteCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetBoardAsync(request.BoardId, request.BanId);
            if (ban == null || ban.BoardId != request.BoardId)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _banRepository.DeleteAsync(ban.BanId);

            await _banCache.RemoveBoardAsync(board.BoardId, ban.BanId);
            await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
            await _banCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banCache.GetBoardCountAsync(board.BoardId);

            await _banAppealCache.RemoveBoardAsync(board.BoardId, ban.BanId);
            await _banAppealCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banAppealCache.GetBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}