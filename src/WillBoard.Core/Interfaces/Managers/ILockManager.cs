using System;
using System.Threading.Tasks;

namespace WillBoard.Core.Interfaces.Managers
{
    public interface ILockManager
    {
        Task<IDisposable> GetAccountLockAsync(string key);
        Task<IDisposable> GetAuthenticationLockAsync(string key);
        Task<IDisposable> GetBanLockAsync(string key);
        Task<IDisposable> GetBanAppealLockAsync(string key);
        Task<IDisposable> GetInvitationLockAsync(string key);
        Task<IDisposable> GetConfigurationLockAsync(string key);
        Task<IDisposable> GetTranslationLockAsync(string key);
        Task<IDisposable> GetAuthorizationLockAsync(string key);
        Task<IDisposable> GetNavigationLockAsync(string key);
        Task<IDisposable> GetBoardLockAsync(string key);
        Task<IDisposable> GetReportLockAsync(string key);
        Task<IDisposable> GetVerificationLockAsync(string key);
        Task<IDisposable> GetBadIpLockAsync(string key);
        Task<IDisposable> GetCountryIpLockAsync(string key);
        Task<IDisposable> GetPostLockAsync(string key);
        Task<IDisposable> GetPostQueuedLockAsync(string key);
        Task<IDisposable> GetBlockListLockAsync(string key);
    }
}