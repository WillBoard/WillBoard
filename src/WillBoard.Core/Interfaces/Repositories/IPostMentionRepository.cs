using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IPostMentionRepository
    {
        Task CreateCollectionAsync(IEnumerable<PostMention> postMentionCollection, IDbConnection dbConnection, IDbTransaction transaction);

        Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId);
        Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction);
        Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, int postId);
        Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction);

        Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId);
        Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction);
        Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, int postId);
        Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction);

        Task UpdateAsync(string incomingBoardId, int incomingPostId, int? incomingThreadId, bool active, IDbConnection dbConnection, IDbTransaction transaction);

        Task DeleteIncomingAsync(string boardId, int postId);
        Task DeleteOutcomingAsync(string boardId, int postId);
    }
}