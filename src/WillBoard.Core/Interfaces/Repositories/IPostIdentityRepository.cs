using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IPostIdentityRepository
    {
        Task CreatePostIdentityAsync(string boardId);

        Task<PostIdentity> ReadPostIdentityAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction);

        Task UpdatePostIdentityAsync(PostIdentity postIdentity, IDbConnection dbConnection, IDbTransaction transaction);

        Task DeletePostIdentityAsync(string boardId);
    }
}