using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardCreate
{
    public class BoardCreateCommandHandler : IRequestHandler<BoardCreateCommand, Status<InternalError>>
    {
        private static readonly IEnumerable<string> ExcludeCollection = new string[] { "Create", "Administration" };

        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBoardRepository _boardRepository;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IPostIdentityRepository _postIdentityRepository;
        private readonly IBoardCache _boardCache;
        private readonly IAuthorizationCache _authorizationCache;
        private readonly IStorageService _storageService;

        public BoardCreateCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IBoardRepository boardRepository, IAuthorizationRepository authorizationRepository, IPostIdentityRepository postIdentityRepository, IBoardCache boardCache, IAuthorizationCache authorizationCache, IStorageService storageService)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _boardRepository = boardRepository;
            _authorizationRepository = authorizationRepository;
            _postIdentityRepository = postIdentityRepository;
            _boardCache = boardCache;
            _authorizationCache = authorizationCache;
            _storageService = storageService;
        }

        public async Task<Status<InternalError>> Handle(BoardCreateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (!Regex.IsMatch(request.BoardId, "^[a-z0-9]{1,32}$"))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvalidBoardId));
            }

            if (ExcludeCollection.Any(e => e == request.BoardId))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorUnavailableBoardId));
            }

            var existingBoard = await _boardCache.GetAsync(request.BoardId);
            if (existingBoard != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorUnavailableBoardId));
            }

            var board = new Core.Entities.Board(request.BoardId);

            var authorization = new Authorization()
            {
                AuthorizationId = Guid.NewGuid(),
                BoardId = request.BoardId,
                AccountId = requestAccount.AccountId,
                Creation = _dateTimeProvider.UtcNow,
                Name = "",

                PermissionReportRead = true,
                PermissionReportDelete = true,

                PermissionBanRead = true,
                PermissionBanCreate = true,
                PermissionBanUpdate = true,
                PermissionBanDelete = true,

                PermissionBanAppealRead = true,
                PermissionBanAppealAccept = true,
                PermissionBanAppealReject = true,

                PermissionIpRead = true,
                PermissionIpDeletePosts = true,

                PermissionPostEdit = true,
                PermissionPostDelete = true,
                PermissionPostDeleteFile = true,

                PermissionThreadReplyLock = true,
                PermissionThreadBumpLock = true,
                PermissionThreadExcessive = true,
                PermissionThreadPin = true,
                PermissionThreadCopy = true,

                PermissionAuthorizationRead = true,
                PermissionAuthorizationUpdate = true,
                PermissionAuthorizationDelete = true,

                PermissionInvitationRead = true,
                PermissionInvitationCreate = true,
                PermissionInvitationUpdate = true,
                PermissionInvitationDelete = true,

                PermissionBoardView = true,
                PermissionBoardUpdate = true,
                PermissionBoardDelete = true,
            };

            await _boardRepository.CreateAsync(board);

            await _authorizationRepository.CreateAsync(authorization);
            await _postIdentityRepository.CreatePostIdentityAsync(board.BoardId);

            _storageService.CreateBoardDirectory(board.BoardId);

            await _boardCache.RemoveCollectionAsync();

            await _authorizationCache.RemoveAccountCollectionAsync(authorization.AccountId);
            await _authorizationCache.PurgeBoardAsync(board.BoardId);
            await _authorizationCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}