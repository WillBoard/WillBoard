using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardAuthorizationUpdate
{
    public class BoardAuthorizationUpdateCommandHandler : IRequestHandler<BoardAuthorizationUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IBoardCache _boardCache;
        private readonly IAuthorizationCache _authorizationCache;

        public BoardAuthorizationUpdateCommandHandler(AccountManager accountManager, IAuthorizationRepository authorizationRepository, IBoardCache boardCache, IAuthorizationCache authorizationCache)
        {
            _accountManager = accountManager;
            _authorizationRepository = authorizationRepository;
            _boardCache = boardCache;
            _authorizationCache = authorizationCache;
        }

        public async Task<Status<InternalError>> Handle(BoardAuthorizationUpdateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionAuthorizationUpdate))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var authorization = await _authorizationCache.GetBoardAsync(request.BoardId, request.AuthorizationId);
            if (authorization == null || authorization.BoardId != request.BoardId)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var authorizationUpdate = new Core.Entities.Authorization()
            {
                AuthorizationId = authorization.AuthorizationId,
                BoardId = authorization.BoardId,
                Name = request.Name,

                PermissionReportRead = request.PermissionReportRead,
                PermissionReportDelete = request.PermissionReportDelete,

                PermissionBanRead = request.PermissionBanRead,
                PermissionBanCreate = request.PermissionBanCreate,
                PermissionBanUpdate = request.PermissionBanUpdate,
                PermissionBanDelete = request.PermissionBanDelete,

                PermissionBanAppealRead = request.PermissionBanAppealRead,
                PermissionBanAppealAccept = request.PermissionBanAppealAccept,
                PermissionBanAppealReject = request.PermissionBanAppealReject,

                PermissionIpRead = request.PermissionIpRead,
                PermissionIpDeletePosts = request.PermissionIpDeletePosts,

                PermissionPostEdit = request.PermissionPostEdit,
                PermissionPostDelete = request.PermissionPostDelete,
                PermissionPostDeleteFile = request.PermissionPostDeleteFile,

                PermissionThreadReplyLock = request.PermissionThreadReplyLock,
                PermissionThreadBumpLock = request.PermissionThreadBumpLock,
                PermissionThreadExcessive = request.PermissionThreadExcessive,
                PermissionThreadPin = request.PermissionThreadPin,
                PermissionThreadCopy = request.PermissionThreadCopy,

                PermissionAuthorizationRead = request.PermissionAuthorizationRead,
                PermissionAuthorizationUpdate = request.PermissionAuthorizationUpdate,
                PermissionAuthorizationDelete = request.PermissionAuthorizationDelete,

                PermissionInvitationRead = request.PermissionInvitationRead,
                PermissionInvitationCreate = request.PermissionInvitationCreate,
                PermissionInvitationUpdate = request.PermissionInvitationUpdate,
                PermissionInvitationDelete = request.PermissionInvitationDelete,

                PermissionBoardView = request.PermissionBoardView,
                PermissionBoardUpdate = request.PermissionBoardUpdate,
                PermissionBoardDelete = request.PermissionBoardDelete,
            };

            await _authorizationRepository.UpdateAsync(authorizationUpdate);

            await _authorizationCache.RemoveAccountCollectionAsync(authorization.AccountId);
            await _authorizationCache.RemoveBoardAsync(board.BoardId, authorization.AuthorizationId);
            await _authorizationCache.PurgeBoardCollectionAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}