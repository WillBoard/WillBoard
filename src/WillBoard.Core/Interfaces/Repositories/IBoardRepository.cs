using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IBoardRepository
    {
        Task CreateAsync(Board board);

        Task<IEnumerable<Board>> ReadCollectionAsync();

        Task UpdateAsync(Board board);

        Task DeleteAsync(string boardId);
    }
}