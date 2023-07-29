using System.Threading;

namespace WillBoard.Core.Interfaces.Managers
{
    public interface ICancellationTokenManager
    {
        CancellationTokenSource GetAccountCancellationTokenSource(string key);
        void RemoveAccountCancellationTokenSource(string key);

        CancellationTokenSource GetAuthenticationCancellationTokenSource(string key);
        void RemoveAuthenticationCancellationTokenSource(string key);

        CancellationTokenSource GetBanCancellationTokenSource(string key);
        void RemoveBanCancellationTokenSource(string key);

        CancellationTokenSource GetBanAppealCancellationTokenSource(string key);
        void RemoveBanAppealCancellationTokenSource(string key);

        CancellationTokenSource GetInvitationCancellationTokenSource(string key);
        void RemoveInvitationCancellationTokenSource(string key);

        CancellationTokenSource GetAuthorizationCancellationTokenSource(string key);
        void RemoveAuthorizationCancellationTokenSource(string key);

        CancellationTokenSource GetNavigationCancellationTokenSource(string key);
        void RemoveNavigationCancellationTokenSource(string key);

        CancellationTokenSource GetBoardCancellationTokenSource(string key);
        void RemoveBoardCancellationTokenSource(string key);

        CancellationTokenSource GetReportCancellationTokenSource(string key);
        void RemoveReportCancellationTokenSource(string key);

        CancellationTokenSource GetVerificationCancellationTokenSource(string key);
        void RemoveVerificationCancellationTokenSource(string key);

        CancellationTokenSource GetBadIpCancellationTokenSource(string key);
        void RemoveBadIpCancellationTokenSource(string key);

        CancellationTokenSource GetCountryIpCancellationTokenSource(string key);
        void RemoveCountryIpCancellationTokenSource(string key);

        CancellationTokenSource GetPostCancellationTokenSource(string key);
        void RemovePostCancellationTokenSource(string key);

        CancellationTokenSource GetBlockListCancellationTokenSource(string key);
        void RemoveBlockListCancellationTokenSource(string key);
    }
}