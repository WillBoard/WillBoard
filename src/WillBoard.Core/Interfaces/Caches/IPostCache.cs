using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IPostCache
    {
        Task<Post> GetAdaptedAsync(Board board, int postId);
        Task AddAdaptedAsync(Board board, Post post);
        Task UpdateAdaptedExcessiveAsync(Board board, int postId, DateTime? excessive);
        Task UpdateAdaptedFileDeletedAsync(Board board, int postId, bool fileDeleted);
        Task UpdateAdaptedAsync(Board board, Post post);
        Task RemoveAdaptedAsync(Board board, int postId);

        Task<IEnumerable<Post>> GetAdaptedCollectionAsync(Board board);
        Task<KeyValuePair<Post, Post[]>> GetAdaptedBoardThreadAsync(Board board, int threadId, int? last = null);
        Task<IDictionary<Post, Post[]>> GetAdaptedBoardAsync(Board board, int page);
        Task<Post[]> GetAdaptedSearchAsync(Board board, int? postId, int? threadId, string message, string file, string type, string order);
        Task<Post> GetAdaptedThreadLastByIpNumberAsync(Board board, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> GetAdaptedReplyLastByIpNumberAsync(Board board, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> GetAdaptedReplyUserIdAsync(Board board, int threadId, IpVersion ipVersion, UInt128 ipNumber);
        Task<Post> GetAdaptedByFileHashAsync(Board board, byte[] fileHash);
        Task<int> GetAdaptedBoardPageMaxAsync(Board board);

        Task PurgeAdaptedCollectionAsync(string boardId);
    }
}