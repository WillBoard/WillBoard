using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IBoardCache
    {
        Task<Board> GetAsync(string boardId);
        Task<IEnumerable<Board>> GetCollectionAsync();
        Task RemoveCollectionAsync();
    }
}