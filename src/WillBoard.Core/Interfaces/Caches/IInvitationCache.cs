using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IInvitationCache
    {
        Task<Invitation> GetAccountAsync(Guid accountId, Guid invitationId);
        Task RemoveAccountAsync(Guid accountId, Guid invitationId);
        Task PurgeAccountAsync(Guid accountId);
        Task PurgeAccountAsync();

        Task<IEnumerable<Invitation>> GetAccountCollectionAsync(Guid accountId, int skip, int take);
        Task PurgeAccountCollectionAsync(Guid accountId);
        Task PurgeAccountCollectionAsync();

        Task<int> GetAccountCountAsync(Guid accountId);
        Task RemoveAccountCountAsync(Guid accountId);
        Task PurgeAccountCountAsync();

        Task<Invitation> GetBoardAsync(string boardId, Guid invitationId);
        Task RemoveBoardAsync(string boardId, Guid invitationId);
        Task PurgeBoardAsync(string boardId);

        Task<IEnumerable<Invitation>> GetBoardCollectionAsync(string boardId, int skip, int take);
        Task PurgeBoardCollectionAsync(string boardId);

        Task<int> GetBoardCountAsync(string boardId);
        Task RemoveBoardCountAsync(string boardId);
    }
}