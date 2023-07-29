using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IAuthorizationCache
    {
        Task<IEnumerable<Authorization>> GetAccountCollectionAsync(Guid accountId);
        Task RemoveAccountCollectionAsync(Guid accountId);
        Task PurgeAccountCollectionAsync();

        Task<Authorization> GetBoardAsync(string boardId, Guid authorizationId);
        Task RemoveBoardAsync(string boardId, Guid authorizationId);
        Task PurgeBoardAsync(string boardId);

        Task<IEnumerable<Authorization>> GetBoardCollectionAsync(string boardId, int skip, int take);
        Task PurgeBoardCollectionAsync(string boardId);

        Task<int> GetBoardCountAsync(string boardId);
        Task RemoveBoardCountAsync(string boardId);
    }
}