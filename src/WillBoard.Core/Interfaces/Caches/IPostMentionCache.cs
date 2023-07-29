using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IPostMentionCache
    {
        Task<IDictionary<string, IEnumerable<PostMention>>> GetIncomingCollectionAsync(string boardId);
        Task UpdateIncomingCollectionAsync(string boardId, string key, IEnumerable<PostMention> postMentionCollection);
        Task RemoveIncomingCollectionAsync(string boardId);

        Task<IDictionary<string, IEnumerable<PostMention>>> GetOutcomingCollectionAsync(string boardId);
        Task UpdateOutcomingCollectionAsync(string boardId, string key, IEnumerable<PostMention> postMentionCollection);
        Task RemoveOutcomingCollectionAsync(string boardId);
    }
}