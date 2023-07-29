using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class PostMentionRepository : IPostMentionRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public PostMentionRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateCollectionAsync(IEnumerable<PostMention> postMentionCollection, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $"INSERT INTO PostMention (OutcomingBoardId, OutcomingPostId, OutcomingThreadId, IncomingBoardId, IncomingPostId, IncomingThreadId, Active) VALUES (@OutcomingBoardId, @OutcomingPostId, @OutcomingThreadId, @IncomingBoardId, @IncomingPostId, (SELECT ThreadId FROM Post WHERE BoardId = @IncomingBoardId AND PostId = @IncomingPostId), (SELECT CASE WHEN EXISTS (SELECT 1 FROM Post WHERE BoardId = @IncomingBoardId AND PostId = @IncomingPostId) THEN 1 ELSE 0 END));";

            await dbConnection.ExecuteAsync(sql, postMentionCollection, transaction: transaction);
        }

        public async Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                return await ReadIncomingCollectionAsync(boardId, dbConnection, null);
            }
        }

        public async Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = @"SELECT * FROM PostMention WHERE IncomingBoardId = @BoardId AND Active = 1;";

            return await dbConnection.QueryAsync<PostMention>(sql, new { BoardId = boardId }, transaction);
        }

        public async Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, int postId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                return await ReadIncomingCollectionAsync(boardId, dbConnection, null);
            }
        }

        public async Task<IEnumerable<PostMention>> ReadIncomingCollectionAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = @"SELECT * FROM PostMention WHERE IncomingBoardId = @BoardId AND IncomingPostId = @PostId AND Active = 1;";

            return await dbConnection.QueryAsync<PostMention>(sql, new { BoardId = boardId, PostId = postId }, transaction);
        }

        public async Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                return await ReadOutcomingCollectionAsync(boardId, dbConnection, null);
            }
        }

        public async Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = @"SELECT * FROM PostMention WHERE OutcomingBoardId = @BoardId AND Active = 1;";

            return await dbConnection.QueryAsync<PostMention>(sql, new { BoardId = boardId }, transaction);
        }

        public async Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, int postId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                return await ReadOutcomingCollectionAsync(boardId, dbConnection, null);
            }
        }

        public async Task<IEnumerable<PostMention>> ReadOutcomingCollectionAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = @"SELECT * FROM PostMention WHERE OutcomingBoardId = @BoardId AND OutcomingPostId = @PostId AND Active = 1;";

            return await dbConnection.QueryAsync<PostMention>(sql, new { BoardId = boardId, PostId = postId }, transaction);
        }

        public async Task UpdateAsync(string incomingBoardId, int incomingPostId, int? incomingThreadId, bool active, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $"UPDATE PostMention  SET Active = @Active, IncomingThreadId = @IncomingThreadId WHERE IncomingBoardId = @IncomingBoardId AND IncomingPostId = @IncomingPostId;";

            await dbConnection.ExecuteAsync(sql, new { Active = active, IncomingThreadId = incomingThreadId, IncomingBoardId = incomingBoardId, IncomingPostId = incomingPostId }, transaction: transaction);
        }

        public async Task DeleteIncomingAsync(string incomingBoardId, int incomingPostId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"DELETE PostMention WHERE IncomingBoardId = @IncomingBoardId AND IncomingPostId = @IncomingPostId;";

                await dbConnection.ExecuteAsync(sql, new { IncomingBoardId = incomingBoardId, IncomingPostId = incomingPostId });
            }
        }

        public async Task DeleteOutcomingAsync(string outcomingBoardId, int outcomingPostId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"DELETE PostMention WHERE OutcomingBoardId = @OutcomingBoardId AND OutcomingPostId = @OutcomingPostId;";

                await dbConnection.ExecuteAsync(sql, new { OutcomingBoardId = outcomingBoardId, OutcomingPostId = outcomingPostId });
            }
        }
    }
}