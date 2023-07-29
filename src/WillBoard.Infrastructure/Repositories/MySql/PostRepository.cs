using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class PostRepository : IPostRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PostRepository(ISqlConnectionService sqlConnectionService, IDateTimeProvider dateTimeProvider)
        {
            _sqlConnectionService = sqlConnectionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task UpdateReplyCountAndBumpAsync(string boardId, int threadId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                await UpdateReplyCountAndBumpAsync(boardId, threadId, dbConnection, null);
            }
        }

        public async Task UpdateReplyCountAndBumpAsync(string boardId, int threadId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $@"UPDATE Post SET Bump = (SELECT Creation FROM Post WHERE (BoardId = @BoardId AND ThreadId = @ThreadId AND Sage = 0) OR (BoardId = @BoardId AND PostId = @ThreadId) ORDER BY PostId DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY), ReplyCount = (SELECT COUNT(PostId) FROM Post WHERE BoardId = @BoardId AND ThreadId = @ThreadId) WHERE BoardId = @BoardId AND PostId = @ThreadId;";

            await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, ThreadId = threadId }, transaction: transaction);
        }

        public async Task CreateAsync(Post post)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                await CreateAsync(post, dbConnection, null);
            }
        }

        public async Task CreateAsync(Post post, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $@"INSERT INTO `Post` (
                            `PostId`,
                            `ThreadId`,
                            `BoardId`,
                            `Creation`,
                            `Subject`,
                            `Email`,
                            `Name`,
                            `MessageRaw`,
                            `MessageStatic`,
                            `File`,
                            `FileSpoiler`,
                            `FileDeleted`,
                            `FileNameOriginal`,
                            `FileName`,
                            `FileMimeType`,
                            `FileHash`,
                            `FileSize`,
                            `FileWidth`,
                            `FileHeight`,
                            `FileDuration`,
                            `FilePreview`,
                            `FilePreviewName`,
                            `FilePreviewWidth`,
                            `FilePreviewHeight`,
                            `Password`,
                            `IpVersion`,
                            `IpNumber`,
                            `Country`,
                            `UserAgent`,
                            `UserId`,
                            `Sage`,
                            `Pin`,
                            `ReplyLock`,
                            `BumpLock`,
                            `Excessive`,
                            `ForceUserId`,
                            `ForceCountry`,
                            `Bump`,
                            `ReplyCount`
                            ) VALUES (
                            @PostId,
                            @ThreadId,
                            @BoardId,
                            @Creation,
                            @Subject,
                            @Email,
                            @Name,
                            @MessageRaw,
                            @MessageStatic,
                            @File,
                            @FileSpoiler,
                            @FileDeleted,
                            @FileNameOriginal,
                            @FileName,
                            @FileMimeType,
                            @FileHash,
                            @FileSize,
                            @FileWidth,
                            @FileHeight,
                            @FileDuration,
                            @FilePreview,
                            @FilePreviewName,
                            @FilePreviewWidth,
                            @FilePreviewHeight,
                            @Password,
                            @IpVersion,
                            @IpNumber,
                            @Country,
                            @UserAgent,
                            @UserId,
                            @Sage,
                            @Pin,
                            @ReplyLock,
                            @BumpLock,
                            @Excessive,
                            @ForceUserId,
                            @ForceCountry,
                            @Bump,
                            @ReplyCount);";

            await dbConnection.ExecuteAsync(sql, post, transaction: transaction);
        }

        public async Task<Post> ReadAsync(string boardId, int postId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                return await ReadAsync(boardId, postId, dbConnection, null);
            }
        }

        public async Task<Post> ReadAsync(string boardId, int postId, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND PostId = @PostId";

            var post = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, PostId = postId }, transaction: transaction);

            if (post == null)
            {
                return post;
            }

            return post;
        }

        public async Task<Post> ReadThreadLastByIpAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId IS NULL AND IpVersion = @IpVersion AND IpNumber = @IpNumber ORDER BY PostId DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

                var result = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, IpVersion = ipVersion, IpNumber = ipNumber });

                return result;
            }
        }

        public async Task<Post> ReadReplyLastByIpAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId IS NOT NULL AND IpVersion = @IpVersion AND IpNumber = @IpNumber ORDER BY PostId DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

                var result = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, IpVersion = ipVersion, IpNumber = ipNumber });

                return result;
            }
        }

        public async Task<Post> ReadReplyLastByIpAndThreadIdAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId IS NOT NULL AND IpVersion = @IpVersion AND IpNumber = @IpNumber ORDER BY PostId DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

                var result = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, IpVersion = ipVersion, IpNumber = ipNumber });

                return result;
            }
        }

        public async Task<Post> ReadByFileHashAsync(string boardId, byte[] fileHash)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND FileHash = @FileHash AND FileDeleted = 0  ORDER BY Creation DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

                var result = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, FileHash = fileHash });

                return result;
            }
        }

        public async Task<Post> ReadReplyUserIdAsync(string boardId, int threadId, IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND (PostId = @threadId OR ThreadId = @threadId) AND IpVersion = @IpVersion AND IpNumber = @IpNumber ORDER BY PostId DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

                var response = await dbConnection.QueryFirstOrDefaultAsync<Post>(sql, new { BoardId = boardId, ThreadId = threadId, IpVersion = ipVersion, IpNumber = ipNumber });

                return response;
            }
        }

        public async Task<IEnumerable<Post>> ReadThreadReplyCollectionAsync(string boardId, int threadId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId = @ThreadId;";

                var threadCollection = await dbConnection.QueryAsync<Post>(sql, new { BoardId = boardId, ThreadId = threadId });

                return threadCollection;
            }
        }

        public async Task<IEnumerable<Post>> ReadUnmarkedExcessiveThreadCollectionAsync(string boardId, int threadSkip)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId IS NULL AND Excessive IS NULL AND Pin = 0 ORDER BY Bump DESC OFFSET @Skip ROWS;";

                var threadCollection = await dbConnection.QueryAsync<Post>(sql, new { BoardId = boardId, Skip = threadSkip });

                return threadCollection;
            }
        }

        public async Task<IEnumerable<Post>> ReadExpiredExcessiveCollectionAsync(string boardId, int threadExcessiveTimeMax)
        {
            var result = new List<Post>();

            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND Excessive IS NOT NULL;";

                var threadCollection = await dbConnection.QueryAsync<Post>(sql, new { BoardId = boardId });

                var expiredThreadCollection = threadCollection.Where(e => e.Excessive != null && e.Excessive.Value.AddSeconds(threadExcessiveTimeMax) < _dateTimeProvider.UtcNow);

                result.AddRange(expiredThreadCollection);

                var expiredThreadIdCollection = expiredThreadCollection.Select(e => e.PostId);

                var sqlReply = $"SELECT * FROM Post WHERE BoardId = @BoardId AND ThreadId IN @threadId";

                var replyCollection = await dbConnection.QueryAsync<Post>(sqlReply, new { BoardId = boardId, threadId = expiredThreadIdCollection });

                result.AddRange(replyCollection);

                return result;
            }
        }

        public async Task<IEnumerable<Post>> ReadUnanonymizedCollectionAsync(string boardId, int anonymizationTimeMax)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var dateTime = _dateTimeProvider.UtcNow;
                dateTime = dateTime.AddSeconds(anonymizationTimeMax * -1);

                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId AND IpNumber != 0 AND Creation < @DateTime;";

                return await dbConnection.QueryAsync<Post>(sql, new { BoardId = boardId, DateTime = dateTime });
            }
        }

        public async Task UpdateAsync(Post post)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET Subject = @Subject, Name = @Name, Email = @Email, MessageRaw = @MessageRaw, MessageStatic = @MessageStatic, Password = @Password WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, post);
            }
        }

        public async Task UpdateFileDeleted(string boardId, int postId, bool fileDeleted)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET FileDeleted = @FileDeleted WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, PostId = postId, FileDeleted = fileDeleted });
            }
        }

        public async Task UpdateExcessiveAsync(string boardId, int threadId, DateTime? excessive)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET Excessive = @Excessive WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, PostId = threadId, Excessive = excessive });
            }
        }

        public async Task UpdateReplyLockAsync(string boardId, int threadId, bool replyLock)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET ReplyLock = @ReplyLock WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, PostId = threadId, ReplyLock = replyLock });
            }
        }

        public async Task UpdateBumpLockAsync(string boardId, int threadId, bool bumpLock)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET BumpLock = @BumpLock WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, PostId = threadId, BumpLock = bumpLock });
            }
        }

        public async Task UpdatePinAsync(string boardId, int threadId, bool pin)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE Post SET Pin = @Pin WHERE BoardId = @BoardId AND PostId = @PostId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BoardId = boardId, PostId = threadId, Pin = pin });
            }
        }

        public async Task DeleteAsync(Post post, IDbConnection dbConnection, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync($@"DELETE FROM Post WHERE PostId = @PostId AND BoardId = @BoardId;", post, transaction);
        }

        public async Task DeleteCollectionAsync(IEnumerable<Post> postCollection, IDbConnection dbConnection, IDbTransaction transaction)
        {
            await dbConnection.ExecuteAsync($@"DELETE FROM Post WHERE PostId = @PostId AND BoardId = @BoardId;", postCollection, transaction);
        }

        public async Task AnonymizeCollectionAsync(IEnumerable<Post> postCollection)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                await dbConnection.ExecuteAsync($@"UPDATE Post SET IpNumber = 0, UserAgent = """", Password = ""{Guid.NewGuid().ToString("N")}"" WHERE BoardId = @BoardId AND PostId = @PostId;", postCollection);
            }
        }

        public async Task<IEnumerable<Post>> ReadCollectionAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM Post WHERE BoardId = @BoardId;";

                var postCollection = await dbConnection.QueryAsync<Post>(sql, new { BoardId = boardId });

                return postCollection;
            }
        }
    }
}