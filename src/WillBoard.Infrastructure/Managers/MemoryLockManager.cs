using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Locks;

namespace WillBoard.Infrastructure.Locks
{
    public class MemoryLockManager : ILockManager
    {
        private static readonly object _accountLock = new object();
        private static readonly List<AccountLock> _accountLockCollection = new List<AccountLock>();

        private static readonly object _authenticationLock = new object();
        private static readonly List<AuthenticationLock> _authenticationLockCollection = new List<AuthenticationLock>();

        private static readonly object _banLock = new object();
        private static readonly List<BanLock> _banLockCollection = new List<BanLock>();

        private static readonly object _banAppealLock = new object();
        private static readonly List<BanAppealLock> _banAppealLockCollection = new List<BanAppealLock>();

        private static readonly object _invitationLock = new object();
        private static readonly List<InvitationLock> _invitationLockCollection = new List<InvitationLock>();

        private static readonly object _configurationLock = new object();
        private static readonly List<ConfigurationLock> _configurationLockCollection = new List<ConfigurationLock>();

        private static readonly object _translationLock = new object();
        private static readonly List<TranslationLock> _translationLockCollection = new List<TranslationLock>();

        private static readonly object _authorizationLock = new object();
        private static readonly List<AuthorizationLock> _authorizationLockCollection = new List<AuthorizationLock>();

        private static readonly object _navigationLock = new object();
        private static readonly List<NavigationLock> _navigationLockCollection = new List<NavigationLock>();

        private static readonly object _boardLock = new object();
        private static readonly List<BoardLock> _boardLockCollection = new List<BoardLock>();

        private static readonly object _reportLock = new object();
        private static readonly List<ReportLock> _reportLockCollection = new List<ReportLock>();

        private static readonly object _verificationLock = new object();
        private static readonly List<VerificationLock> _verificationLockCollection = new List<VerificationLock>();

        private static readonly object _badIpLock = new object();
        private static readonly List<BadIpLock> _badIpLockCollection = new List<BadIpLock>();

        private static readonly object _countryIpLock = new object();
        private static readonly List<CountryIpLock> _countryIpLockCollection = new List<CountryIpLock>();

        private static readonly object _postLock = new object();
        private static readonly List<PostLock> _postLockCollection = new List<PostLock>();

        private static readonly object _postQueuedLock = new object();
        private static readonly List<PostQueuedLock> _postQueuedLockCollection = new List<PostQueuedLock>();

        private static readonly object _blockListLock = new object();
        private static readonly List<BlockListLock> _blockListLockCollection = new List<BlockListLock>();

        public async Task<IDisposable> GetAccountLockAsync(string key)
        {
            var objectLock = new AccountLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetAuthenticationLockAsync(string key)
        {
            var objectLock = new AuthenticationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetBanLockAsync(string key)
        {
            var objectLock = new BanLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetBanAppealLockAsync(string key)
        {
            var objectLock = new BanAppealLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetInvitationLockAsync(string key)
        {
            var objectLock = new InvitationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetConfigurationLockAsync(string key)
        {
            var objectLock = new ConfigurationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetTranslationLockAsync(string key)
        {
            var objectLock = new TranslationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetAuthorizationLockAsync(string key)
        {
            var objectLock = new AuthorizationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetNavigationLockAsync(string key)
        {
            var objectLock = new NavigationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetBoardLockAsync(string key)
        {
            var objectLock = new BoardLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetReportLockAsync(string key)
        {
            var objectLock = new ReportLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetVerificationLockAsync(string key)
        {
            var objectLock = new VerificationLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetBadIpLockAsync(string key)
        {
            var objectLock = new BadIpLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetCountryIpLockAsync(string key)
        {
            var objectLock = new CountryIpLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetPostLockAsync(string key)
        {
            var objectLock = new PostLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetPostQueuedLockAsync(string key)
        {
            var objectLock = new PostQueuedLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        public async Task<IDisposable> GetBlockListLockAsync(string key)
        {
            var objectLock = new BlockListLock(key);
            await objectLock.WaitAsync();
            return objectLock;
        }

        private class AccountLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public AccountLock(string key)
            {
                _key = key;
                lock (_accountLock)
                {
                    var existing = _accountLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _accountLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_accountLock)
                {
                    _accountLockCollection.Remove(this);
                }
            }
        }

        private class AuthenticationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public AuthenticationLock(string key)
            {
                _key = key;
                lock (_authenticationLock)
                {
                    var existing = _authenticationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _authenticationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_authenticationLock)
                {
                    _authenticationLockCollection.Remove(this);
                }
            }
        }

        private class BanLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public BanLock(string key)
            {
                _key = key;
                lock (_banLock)
                {
                    var existing = _banLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _banLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_banLock)
                {
                    _banLockCollection.Remove(this);
                }
            }
        }

        private class BanAppealLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public BanAppealLock(string key)
            {
                _key = key;
                lock (_banAppealLock)
                {
                    var existing = _banAppealLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _banAppealLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_banAppealLock)
                {
                    _banAppealLockCollection.Remove(this);
                }
            }
        }

        private class InvitationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public InvitationLock(string key)
            {
                _key = key;
                lock (_invitationLock)
                {
                    var existing = _invitationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _invitationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_invitationLock)
                {
                    _invitationLockCollection.Remove(this);
                }
            }
        }

        private class ConfigurationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public ConfigurationLock(string key)
            {
                _key = key;
                lock (_configurationLock)
                {
                    var existing = _configurationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _configurationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_configurationLock)
                {
                    _configurationLockCollection.Remove(this);
                }
            }
        }

        private class TranslationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public TranslationLock(string key)
            {
                _key = key;
                lock (_translationLock)
                {
                    var existing = _translationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _translationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_translationLock)
                {
                    _translationLockCollection.Remove(this);
                }
            }
        }

        private class AuthorizationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public AuthorizationLock(string key)
            {
                _key = key;
                lock (_authorizationLock)
                {
                    var existing = _authorizationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _authorizationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_authorizationLock)
                {
                    _authorizationLockCollection.Remove(this);
                }
            }
        }

        private class NavigationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public NavigationLock(string key)
            {
                _key = key;
                lock (_navigationLock)
                {
                    var existing = _navigationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _navigationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_navigationLock)
                {
                    _navigationLockCollection.Remove(this);
                }
            }
        }

        private class BoardLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public BoardLock(string key)
            {
                _key = key;
                lock (_boardLock)
                {
                    var existing = _boardLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _boardLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_boardLock)
                {
                    _boardLockCollection.Remove(this);
                }
            }
        }

        private class ReportLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public ReportLock(string key)
            {
                _key = key;
                lock (_reportLock)
                {
                    var existing = _reportLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _reportLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_reportLock)
                {
                    _reportLockCollection.Remove(this);
                }
            }
        }

        private class VerificationLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public VerificationLock(string key)
            {
                _key = key;
                lock (_verificationLock)
                {
                    var existing = _verificationLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _verificationLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_verificationLock)
                {
                    _verificationLockCollection.Remove(this);
                }
            }
        }

        private class BadIpLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public BadIpLock(string key)
            {
                _key = key;
                lock (_badIpLock)
                {
                    var existing = _badIpLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _badIpLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_badIpLock)
                {
                    _badIpLockCollection.Remove(this);
                }
            }
        }

        private class CountryIpLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public CountryIpLock(string key)
            {
                _key = key;
                lock (_countryIpLock)
                {
                    var existing = _countryIpLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _countryIpLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_countryIpLock)
                {
                    _countryIpLockCollection.Remove(this);
                }
            }
        }

        private class PostLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public PostLock(string key)
            {
                _key = key;
                lock (_postLock)
                {
                    var existing = _postLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _postLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_postLock)
                {
                    _postLockCollection.Remove(this);
                }
            }
        }

        private class PostQueuedLock : IDisposable
        {
            private readonly string _key;
            private readonly QueuedSemaphoreSlim _semaphoreSlim;

            public PostQueuedLock(string key)
            {
                _key = key;
                lock (_postQueuedLock)
                {
                    var existing = _postQueuedLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new QueuedSemaphoreSlim(1) : existing._semaphoreSlim;
                    _postQueuedLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_postQueuedLock)
                {
                    _postQueuedLockCollection.Remove(this);
                }
            }
        }

        private class BlockListLock : IDisposable
        {
            private readonly string _key;
            private readonly SemaphoreSlim _semaphoreSlim;

            public BlockListLock(string key)
            {
                _key = key;
                lock (_blockListLock)
                {
                    var existing = _blockListLockCollection.Find(l => l._key == _key);
                    _semaphoreSlim = existing == null ? new SemaphoreSlim(1, 1) : existing._semaphoreSlim;
                    _blockListLockCollection.Add(this);
                }
            }

            public async Task WaitAsync()
            {
                await _semaphoreSlim.WaitAsync();
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();

                lock (_blockListLock)
                {
                    _blockListLockCollection.Remove(this);
                }
            }
        }
    }
}