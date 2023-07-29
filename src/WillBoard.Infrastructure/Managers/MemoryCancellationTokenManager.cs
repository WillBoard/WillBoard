using System.Collections.Generic;
using System.Threading;
using WillBoard.Core.Interfaces.Managers;

namespace WillBoard.Infrastructure.Managers
{
    public class MemoryCancellationTokenManager : ICancellationTokenManager
    {
        private static readonly object _accountCancellationTokenSourceLock = new object();
        private static readonly object _authenticationCancellationTokenSourceLock = new object();
        private static readonly object _banCancellationTokenSourceLock = new object();
        private static readonly object _banAppealCancellationTokenSourceLock = new object();
        private static readonly object _invitationCancellationTokenSourceLock = new object();
        private static readonly object _authorizationCancellationTokenSourceLock = new object();
        private static readonly object _navigationCancellationTokenSourceLock = new object();
        private static readonly object _boardCancellationTokenSourceLock = new object();
        private static readonly object _reportCancellationTokenSourceLock = new object();
        private static readonly object _verificationCancellationTokenSourceLock = new object();
        private static readonly object _badIpCancellationTokenSourceLock = new object();
        private static readonly object _countryIpCancellationTokenSourceLock = new object();
        private static readonly object _postCancellationTokenSourceLock = new object();
        private static readonly object _blockListCancellationTokenSourceLock = new object();

        private static readonly IDictionary<string, CancellationTokenSource> _accountCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _authenticationCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _banCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _banAppealCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _invitationCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _authorizationCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _navigationCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _boardCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _reportCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _verificationCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _badIpCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _countryIpCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _postCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();
        private static readonly IDictionary<string, CancellationTokenSource> _blockListCancellationTokenSourceCollection = new Dictionary<string, CancellationTokenSource>();

        public CancellationTokenSource GetAccountCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_accountCancellationTokenSourceCollection, _accountCancellationTokenSourceLock, key);
        }

        public void RemoveAccountCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_accountCancellationTokenSourceCollection, _accountCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetAuthenticationCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_authenticationCancellationTokenSourceCollection, _authenticationCancellationTokenSourceLock, key);
        }

        public void RemoveAuthenticationCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_authenticationCancellationTokenSourceCollection, _authenticationCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetBanCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_banCancellationTokenSourceCollection, _banCancellationTokenSourceLock, key);
        }

        public void RemoveBanCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_banCancellationTokenSourceCollection, _banCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetBanAppealCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_banAppealCancellationTokenSourceCollection, _banAppealCancellationTokenSourceLock, key);
        }

        public void RemoveBanAppealCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_banAppealCancellationTokenSourceCollection, _banAppealCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetInvitationCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_invitationCancellationTokenSourceCollection, _invitationCancellationTokenSourceLock, key);
        }

        public void RemoveInvitationCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_invitationCancellationTokenSourceCollection, _invitationCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetAuthorizationCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_authorizationCancellationTokenSourceCollection, _authorizationCancellationTokenSourceLock, key);
        }

        public void RemoveAuthorizationCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_authorizationCancellationTokenSourceCollection, _authorizationCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetNavigationCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_navigationCancellationTokenSourceCollection, _navigationCancellationTokenSourceLock, key);
        }

        public void RemoveNavigationCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_navigationCancellationTokenSourceCollection, _navigationCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetBoardCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_boardCancellationTokenSourceCollection, _boardCancellationTokenSourceLock, key);
        }

        public void RemoveBoardCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_boardCancellationTokenSourceCollection, _boardCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetReportCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_reportCancellationTokenSourceCollection, _reportCancellationTokenSourceLock, key);
        }

        public void RemoveReportCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_reportCancellationTokenSourceCollection, _reportCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetVerificationCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_verificationCancellationTokenSourceCollection, _verificationCancellationTokenSourceLock, key);
        }

        public void RemoveVerificationCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_verificationCancellationTokenSourceCollection, _verificationCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetBadIpCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_badIpCancellationTokenSourceCollection, _badIpCancellationTokenSourceLock, key);
        }

        public void RemoveBadIpCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_badIpCancellationTokenSourceCollection, _badIpCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetCountryIpCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_countryIpCancellationTokenSourceCollection, _countryIpCancellationTokenSourceLock, key);
        }

        public void RemoveCountryIpCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_countryIpCancellationTokenSourceCollection, _countryIpCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetPostCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_postCancellationTokenSourceCollection, _postCancellationTokenSourceLock, key);
        }

        public void RemovePostCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_postCancellationTokenSourceCollection, _postCancellationTokenSourceLock, key);
        }

        public CancellationTokenSource GetBlockListCancellationTokenSource(string key)
        {
            return GetCancellationTokenSource(_blockListCancellationTokenSourceCollection, _blockListCancellationTokenSourceLock, key);
        }

        public void RemoveBlockListCancellationTokenSource(string key)
        {
            RemoveCancellationTokenSource(_blockListCancellationTokenSourceCollection, _blockListCancellationTokenSourceLock, key);
        }

        private CancellationTokenSource GetCancellationTokenSource(IDictionary<string, CancellationTokenSource> cancellationTokenSourceCollection, object cancellationTokenSourceLock, string key)
        {
            if (cancellationTokenSourceCollection.TryGetValue(key, out var cancellationTokenSource))
            {
                return cancellationTokenSource;
            }

            lock (cancellationTokenSourceLock)
            {
                if (cancellationTokenSourceCollection.TryGetValue(key, out cancellationTokenSource))
                {
                    return cancellationTokenSource;
                }

                cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSourceCollection.Add(key, cancellationTokenSource);

                return cancellationTokenSource;
            }
        }

        private void RemoveCancellationTokenSource(IDictionary<string, CancellationTokenSource> cancellationTokenSourceCollection, object cancellationTokenSourceLock, string key)
        {
            lock (cancellationTokenSourceLock)
            {
                if (!cancellationTokenSourceCollection.TryGetValue(key, out var cancellationTokenSource))
                {
                    return;
                }

                if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested && cancellationTokenSource.Token.CanBeCanceled)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }

                cancellationTokenSourceCollection.Remove(key);
            }
        }
    }
}