using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IPostService
    {
        Task<int> CreateAsync(Post post);
        Task DeleteCollectionAsync(IEnumerable<Post> postCollection);
        Task DeleteAsync(Post post);
        Task UpdateMentionCollectionAsync(Post post);
        void Adapt(Board board, Post post);
    }
}