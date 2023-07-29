using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IInvitationRepository
    {
        Task CreateAsync(Invitation invitation);

        Task<Invitation> ReadAccountAsync(Guid accountId, Guid invitationId);
        Task<IEnumerable<Invitation>> ReadAccountCollectionAsync(Guid accountId, int skip, int take);
        Task<int> ReadAccountCountAsync(Guid accountId);

        Task<Invitation> ReadBoardAsync(string boardId, Guid invitationId);
        Task<IEnumerable<Invitation>> ReadBoardCollectionAsync(string boardId, int skip, int take);
        Task<int> ReadBoardCountAsync(string boardId);

        Task UpdateAsync(Invitation invitation);

        Task DeleteAsync(Guid invitationId);
    }
}