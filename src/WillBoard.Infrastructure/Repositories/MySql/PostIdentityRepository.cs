using Dapper;
using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class PostIdentityRepository : IPostIdentityRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public PostIdentityRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreatePostIdentityAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO PostIdentity (BoardId, Number) VALUES (@BoardId, @Number);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, Number = 0 });
            }
        }

        public async Task<PostIdentity> ReadPostIdentityAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $"SELECT * FROM PostIdentity WHERE BoardId = @BoardId";

            return await dbConnection.QuerySingleAsync<PostIdentity>(sql, new { BoardId = boardId }, transaction: transaction);
        }

        public async Task UpdatePostIdentityAsync(PostIdentity postIdentity, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $"UPDATE PostIdentity SET Number = @Number WHERE BoardId = @BoardId;";

            await dbConnection.ExecuteAsync(sql, postIdentity, transaction: transaction);
        }

        public async Task DeletePostIdentityAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM PostIdentity WHERE BoardId = @BoardId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId });
            }
        }
    }
}