using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardDelete
{
    public class BoardDeleteCommandHandler : IRequestHandler<BoardDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardRepository _boardRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;
        private readonly IInvitationCache _invitationCache;
        private readonly IAuthorizationCache _authorizationCache;
        private readonly IReportCache _reportCache;
        private readonly IVerificationCache _verificationCache;
        private readonly IPostCache _postCache;
        private readonly IStorageService _storageService;

        public BoardDeleteCommandHandler(
            AccountManager accountManager,
            IBoardRepository boardRepository,
            IBoardCache boardCache,
            IBanCache banCache,
            IBanAppealCache banAppealCache,
            IInvitationCache invitationCache,
            IAuthorizationCache authorizationCache,
            IReportCache reportCache,
            IVerificationCache verificationCache,
            IPostCache postCache,
            IStorageService storageService)
        {
            _accountManager = accountManager;
            _boardRepository = boardRepository;
            _boardCache = boardCache;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
            _invitationCache = invitationCache;
            _authorizationCache = authorizationCache;
            _reportCache = reportCache;
            _verificationCache = verificationCache;
            _postCache = postCache;
            _storageService = storageService;
        }

        public async Task<Status<InternalError>> Handle(BoardDeleteCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBoardDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            await _boardRepository.DeleteAsync(board.BoardId);

            _storageService.DeleteBoardDirectory(board.BoardId);

            await _postCache.PurgeAdaptedCollectionAsync(board.BoardId);

            await _boardCache.RemoveCollectionAsync();

            await _authorizationCache.PurgeAccountCollectionAsync();
            await _authorizationCache.PurgeBoardAsync(board.BoardId);
            await _authorizationCache.RemoveBoardCountAsync(board.BoardId);

            await _invitationCache.PurgeAccountAsync();
            await _invitationCache.PurgeAccountCollectionAsync();
            await _invitationCache.PurgeAccountCountAsync();
            await _invitationCache.PurgeBoardAsync(board.BoardId);
            await _invitationCache.PurgeBoardCollectionAsync(board.BoardId);
            await _invitationCache.RemoveBoardCountAsync(board.BoardId);

            await _banCache.PurgeBoardAsync(board.BoardId);
            await _banCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
            await _banCache.RemoveBoardCountAsync(board.BoardId);

            await _banAppealCache.PurgeBoardAsync(board.BoardId);
            await _banAppealCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banAppealCache.RemoveBoardCountAsync(board.BoardId);

            await _reportCache.PurgeBoardAsync(board.BoardId);
            await _reportCache.PurgeBoardIpCollectionAsync(board.BoardId);
            await _reportCache.PurgeBoardCollectionAsync(board.BoardId);
            await _reportCache.RemoveBoardCountAsync(board.BoardId);

            await _verificationCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}