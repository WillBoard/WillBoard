using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.CacheClear
{
    public class CacheClearCommandHandler : IRequestHandler<CacheClearCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAccountCache _accountCache;
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IAuthorizationCache _authorizationCache;
        private readonly IBadIpCache _badIpCache;
        private readonly IBanAppealCache _banAppealCache;
        private readonly IBanCache _banCache;
        private readonly IBoardCache _boardCache;
        private readonly IConfigurationCache _configurationCache;
        private readonly ICountryIpCache _countryIpCache;
        private readonly IInvitationCache _invitationCache;
        private readonly INavigationCache _navigationCache;
        private readonly IPostCache _postCache;
        private readonly IReportCache _reportCache;
        private readonly ITranslationCache _translationCache;
        private readonly IVerificationCache _verificationCache;
        private readonly IBlockListCache _blockListCache;

        public CacheClearCommandHandler(
            AccountManager accountManager,
            IAccountCache accountCache,
            IAuthenticationCache authenticationCache,
            IAuthorizationCache authorizationCache,
            IBadIpCache badIpCache,
            IBanAppealCache banAppealCache,
            IBanCache banCache,
            IBoardCache boardCache,
            IConfigurationCache configurationCache,
            ICountryIpCache countryIpCache,
            IInvitationCache invitationCache,
            INavigationCache navigationCache,
            IPostCache postCache,
            IReportCache reportCache,
            ITranslationCache translationCache,
            IVerificationCache verificationCache,
            IBlockListCache blockListCache
            )
        {
            _accountManager = accountManager;
            _accountCache = accountCache;
            _authenticationCache = authenticationCache;
            _authorizationCache = authorizationCache;
            _badIpCache = badIpCache;
            _banAppealCache = banAppealCache;
            _banCache = banCache;
            _boardCache = boardCache;
            _configurationCache = configurationCache;
            _countryIpCache = countryIpCache;
            _invitationCache = invitationCache;
            _navigationCache = navigationCache;
            _postCache = postCache;
            _reportCache = reportCache;
            _translationCache = translationCache;
            _verificationCache = verificationCache;
            _blockListCache = blockListCache;
        }

        public async Task<Status<InternalError>> Handle(CacheClearCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var boardCollection = await _boardCache.GetCollectionAsync();

            if (request.Account)
            {
                await _accountCache.PurgeAsync();
                await _accountCache.PurgeCollectionAsync();
                await _accountCache.RemoveCountAsync();
            }

            if (request.Authentication && request.AuthenticationAccountId is not null)
            {
                await _authenticationCache.PurgeAsync(request.AuthenticationAccountId.Value);
                await _authenticationCache.PurgeCollectionAsync(request.AuthenticationAccountId.Value);
                await _authenticationCache.RemoveCountAsync(request.AuthenticationAccountId.Value);
            }

            if (request.Authorization)
            {
                await _authorizationCache.PurgeAccountCollectionAsync();

                foreach (var board in boardCollection)
                {
                    await _authorizationCache.PurgeBoardAsync(board.BoardId);
                    await _authorizationCache.PurgeBoardCollectionAsync(board.BoardId);
                    await _authorizationCache.RemoveBoardCountAsync(board.BoardId);
                }
            }

            if (request.BadIp)
            {
                await _badIpCache.PurgeAsync();
            }

            if (request.BanAppeal)
            {
                await _banAppealCache.PurgeSystemAsync();
                await _banAppealCache.PurgeSystemBanCollectionAsync();
                await _banAppealCache.PurgeSystemCollectionAsync();
                await _banAppealCache.RemoveSystemCountAsync();

                foreach (var board in boardCollection)
                {
                    await _banAppealCache.PurgeBoardAsync(board.BoardId);
                    await _banAppealCache.PurgeBoardBanCollectionAsync(board.BoardId);
                    await _banAppealCache.PurgeBoardCollectionAsync(board.BoardId);
                    await _banAppealCache.RemoveBoardCountAsync(board.BoardId);
                }
            }

            if (request.Ban)
            {
                await _banCache.PurgeSystemAsync();
                await _banCache.PurgeSystemUnexpiredCollectionAsync();
                await _banCache.PurgeSystemCollectionAsync();
                await _banCache.RemoveSystemCountAsync();

                foreach (var board in boardCollection)
                {
                    await _banCache.PurgeBoardAsync(board.BoardId);
                    await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
                    await _banCache.PurgeBoardCollectionAsync(board.BoardId);
                    await _banCache.RemoveBoardCountAsync(board.BoardId);
                }
            }

            if (request.Board)
            {
                await _boardCache.RemoveCollectionAsync();
            }

            if (request.Configuration)
            {
                await _configurationCache.RemoveCollectionAsync();
            }

            if (request.CountryIp)
            {
                await _countryIpCache.PurgeAsync();
            }

            if (request.Invitation)
            {
                await _invitationCache.PurgeAccountAsync();
                await _invitationCache.PurgeAccountCollectionAsync();
                await _invitationCache.PurgeAccountCountAsync();

                foreach (var board in boardCollection)
                {
                    await _invitationCache.PurgeBoardAsync(board.BoardId);
                    await _invitationCache.PurgeBoardCollectionAsync(board.BoardId);
                    await _invitationCache.RemoveBoardCountAsync(board.BoardId);
                }
            }

            if (request.Navigation)
            {
                await _navigationCache.RemoveCollectionAsync();
            }

            if (request.Post)
            {
                foreach (var board in boardCollection)
                {
                    await _postCache.PurgeAdaptedCollectionAsync(board.BoardId);
                }
            }

            if (request.Report)
            {
                await _reportCache.PurgeSystemAsync();
                await _reportCache.PurgeSystemIpCollectionAsync();
                await _reportCache.PurgeSystemCollectionAsync();
                await _reportCache.RemoveSystemCountAsync();

                foreach (var board in boardCollection)
                {
                    await _reportCache.PurgeBoardAsync(board.BoardId);
                    await _reportCache.PurgeBoardIpCollectionAsync(board.BoardId);
                    await _reportCache.PurgeBoardCollectionAsync(board.BoardId);
                    await _reportCache.RemoveBoardCountAsync(board.BoardId);
                }
            }

            if (request.Translation && !string.IsNullOrEmpty(request.TranslationLanguage))
            {
                await _translationCache.RemoveDictionaryAsync(request.TranslationLanguage);
            }

            if (request.Verification)
            {
                await _verificationCache.PurgeSystemUnexpiredCollectionAsync();

                foreach (var board in boardCollection)
                {
                    await _verificationCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
                }
            }

            if (request.BlockList)
            {
                foreach (var board in boardCollection)
                {
                    await _blockListCache.PurgeBoardDnsBlockListIpVersion4Async(board.BoardId);
                    await _blockListCache.PurgeBoardDnsBlockListIpVersion6Async(board.BoardId);
                    await _blockListCache.PurgeBoardApiBlockListIpVersion4Async(board.BoardId);
                    await _blockListCache.PurgeBoardApiBlockListIpVersion6Async(board.BoardId);
                }
            }

            return Status<InternalError>.SuccessStatus();
        }
    }
}