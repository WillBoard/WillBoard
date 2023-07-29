using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IAuthorizationRepository
    {
        Task CreateAsync(Authorization authorization);

        Task<IEnumerable<Authorization>> ReadAccountCollectionAsync(Guid accountId);
        Task<Authorization> ReadBoardAsync(string boardId, Guid authorizationId);

        Task<IEnumerable<Authorization>> ReadBoardCollectionAsync(string boardId, int skip, int take);
        Task<int> ReadBoardCountAsync(string boardId);

        Task UpdateAsync(Authorization authorization);

        Task DeleteAsync(Guid authorizationId);
    }
}