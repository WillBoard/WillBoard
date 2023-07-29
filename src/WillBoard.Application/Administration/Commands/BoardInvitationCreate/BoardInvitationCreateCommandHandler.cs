using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardInvitationCreate
{
    public class BoardInvitationCreateCommandHandler : IRequestHandler<BoardInvitationCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IBoardCache _boardCache;
        private readonly IAccountCache _accountCache;
        private readonly IConfigurationCache _configurationCache;
        private readonly IInvitationCache _invitationCache;

        public BoardInvitationCreateCommandHandler(
            AccountManager accountManager,
            IDateTimeProvider dateTimeProvider,
            IInvitationRepository invitationRepository,
            IBoardCache boardCache,
            IAccountCache accountCache,
            IConfigurationCache configurationCache,
            IInvitationCache invitationCache)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _invitationRepository = invitationRepository;
            _boardCache = boardCache;
            _accountCache = accountCache;
            _configurationCache = configurationCache;
            _invitationCache = invitationCache;
        }

        public async Task<Status<InternalError>> Handle(BoardInvitationCreateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionInvitationCreate))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var user = await _accountCache.GetAsync(request.AccountId);

            if (user == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!user.Active)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (string.IsNullOrEmpty(request.Message))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvitationFieldMessageRequirement));
            }

            var invitationLengthMin = await _configurationCache.GetAsync(ConfigurationKey.InvitationLengthMin);
            if (request.Message.Length < invitationLengthMin.Value)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvitationFieldMessageLengthMin));
            }

            var invitationLengthMax = await _configurationCache.GetAsync(ConfigurationKey.InvitationLengthMax);
            if (request.Message.Length > invitationLengthMax.Value)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvitationFieldMessageLengthMax));
            }

            var count = await _invitationCache.GetBoardCountAsync(board.BoardId);
            var invitationBoardMax = await _configurationCache.GetAsync(ConfigurationKey.InvitationBoardMax);
            if (count >= invitationBoardMax.Value)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvitationBoardMax));
            }

            var invitation = new Invitation()
            {
                InvitationId = Guid.NewGuid(),
                AccountId = user.AccountId,
                BoardId = board.BoardId,
                Creation = _dateTimeProvider.UtcNow,
                Message = request.Message
            };

            await _invitationRepository.CreateAsync(invitation);

            await _invitationCache.PurgeAccountAsync(invitation.AccountId);
            await _invitationCache.PurgeAccountCollectionAsync(invitation.AccountId);
            await _invitationCache.RemoveAccountCountAsync(invitation.AccountId);

            await _invitationCache.PurgeBoardAsync(board.BoardId);
            await _invitationCache.PurgeBoardCollectionAsync(board.BoardId);
            await _invitationCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}