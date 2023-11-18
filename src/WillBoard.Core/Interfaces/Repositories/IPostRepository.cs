using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync(Post post);
        Task CreateAsync(Post post, IDbConnection dbConnection, IDbTransaction transaction);

        Task<Post> ReadAsync(string boardId, int postId);
        Task<Post> ReadAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction);
        Task<Post> ReadThreadLastByIpAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> ReadReplyLastByIpAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> ReadReplyLastByIpAndThreadIdAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> ReadReplyUserIdAsync(string boardId, int threadId, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> ReadByFileHashAsync(string boardId, byte[] fileHash);

        Task<IEnumerable<Post>> ReadThreadReplyCollectionAsync(string boardId, int threadId);
        Task<IEnumerable<Post>> ReadUnmarkedExcessiveThreadCollectionAsync(string boardId, int threadSkip);
        Task<IEnumerable<Post>> ReadExpiredExcessiveCollectionAsync(string boardId, int threadExcessiveTimeMax);
        Task<IEnumerable<Post>> ReadUnanonymizedCollectionAsync(string boardId, int anonymizationTimeMax);

        Task UpdateAsync(Post post);
        Task UpdateFileDeleted(string boardId, int threadId, bool fileDeleted);
        Task UpdateExcessiveAsync(string boardId, int threadId, DateTime? excessive);
        Task UpdateReplyLockAsync(string boardId, int threadId, bool replyLock);
        Task UpdateBumpLockAsync(string boardId, int threadId, bool bumpLock);
        Task UpdatePinAsync(string boardId, int threadId, bool pin);

        Task UpdateReplyCountAndBumpAsync(string boardId, int threadId);
        Task UpdateReplyCountAndBumpAsync(string boardId, int threadId, IDbConnection dbConnection, IDbTransaction transaction);

        Task AnonymizeCollectionAsync(IEnumerable<Post> postCollection);

        Task DeleteAsync(Post post, IDbConnection dbConnection, IDbTransaction transaction);
        Task DeleteCollectionAsync(IEnumerable<Post> postCollection, IDbConnection dbConnection, IDbTransaction transaction);

        Task<IEnumerable<Post>> ReadCollectionAsync(string boardId);
    }
}