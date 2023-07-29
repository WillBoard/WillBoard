using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Services;

namespace WillBoard.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly IConfigurationService _configurationService;
        private readonly ISqlConnectionService _sqlConnectionService;
        private readonly IPostRepository _postRepository;
        private readonly IPostIdentityRepository _postIdentityRepository;
        private readonly IPostMentionRepository _postMentionRepository;
        private readonly IBoardCache _boardCache;
        private readonly MarkupService _markupService;

        public PostService(
            IConfigurationService instanceService,
            ISqlConnectionService sqlConnectionService,
            IPostRepository postRepository,
            IPostIdentityRepository postIdentityRepository,
            IPostMentionRepository postMentionRepository,
            IBoardCache boardCache,
            MarkupService markupService)
        {
            _configurationService = instanceService;
            _sqlConnectionService = sqlConnectionService;
            _postRepository = postRepository;
            _postIdentityRepository = postIdentityRepository;
            _postMentionRepository = postMentionRepository;
            _boardCache = boardCache;
            _markupService = markupService;
        }

        public async Task<int> CreateAsync(Post post)
        {
            using (IDbConnection dbConnection = _sqlConnectionService.Connection)
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        var postIdentity = await _postIdentityRepository.ReadPostIdentityAsync(post.BoardId, dbConnection, transaction);

                        postIdentity.Number++;
                        post.PostId = postIdentity.Number;

                        await _postRepository.CreateAsync(post, dbConnection, transaction);

                        if (post.ThreadId != null)
                        {
                            await _postRepository.UpdateReplyCountAndBumpAsync(post.BoardId, post.ThreadId.Value, dbConnection, transaction);
                        }

                        foreach (var postMention in post.OutcomingPostMentionCollection)
                        {
                            postMention.OutcomingPostId = post.PostId;
                            postMention.OutcomingBoardId = post.BoardId;
                            postMention.OutcomingThreadId = post.ThreadId;
                        }

                        await _postMentionRepository.CreateCollectionAsync(post.OutcomingPostMentionCollection, dbConnection, transaction: transaction);

                        await _postMentionRepository.UpdateAsync(post.BoardId, post.PostId, post.ThreadId, true, dbConnection, transaction: transaction);

                        await _postIdentityRepository.UpdatePostIdentityAsync(postIdentity, dbConnection, transaction);

                        var outcomingPostMentionCollection = await _postMentionRepository.ReadOutcomingCollectionAsync(post.BoardId, post.PostId, dbConnection, transaction);
                        var incomingPostMentionCollection = await _postMentionRepository.ReadIncomingCollectionAsync(post.BoardId, post.PostId, dbConnection, transaction);

                        post.OutcomingPostMentionCollection = outcomingPostMentionCollection;
                        post.IncomingPostMentionCollection = incomingPostMentionCollection;

                        transaction.Commit();

                        return post.PostId;
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        throw new TransactionException("Exception occured during transaction.", exception);
                    }
                }
            }
        }

        public async Task DeleteCollectionAsync(IEnumerable<Post> postCollection)
        {
            using (IDbConnection dbConnection = _sqlConnectionService.Connection)
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        postCollection = postCollection.OrderByDescending(e => e.ThreadId);

                        await _postRepository.DeleteCollectionAsync(postCollection, dbConnection, transaction);

                        foreach (var post in postCollection)
                        {
                            await _postMentionRepository.UpdateAsync(post.BoardId, post.PostId, post.ThreadId, false, dbConnection, transaction: transaction);
                        }

                        var threadCollection = postCollection.Select(e => new Tuple<string, int?>(e.BoardId, e.ThreadId)).Where(e => e.Item2 != null).Distinct();
                        foreach (var threadId in threadCollection)
                        {
                            await _postRepository.UpdateReplyCountAndBumpAsync(threadId.Item1, threadId.Item2.Value, dbConnection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        throw new TransactionException("Exception occured during transaction.", exception);
                    }
                }
            }
        }

        public async Task DeleteAsync(Post post)
        {
            using (IDbConnection dbConnection = _sqlConnectionService.Connection)
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        await _postRepository.DeleteAsync(post, dbConnection, transaction);
                        await _postMentionRepository.UpdateAsync(post.BoardId, post.PostId, post.ThreadId, false, dbConnection, transaction: transaction);

                        if (post.ThreadId != null)
                        {
                            await _postRepository.UpdateReplyCountAndBumpAsync(post.BoardId, post.ThreadId.Value, dbConnection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        throw new TransactionException("Exception occured during transaction.", exception);
                    }
                }
            }
        }

        public async Task UpdateMentionCollectionAsync(Post post)
        {
            using (IDbConnection dbConnection = _sqlConnectionService.Connection)
            {
                var outcomingPostMentionCollection = await _postMentionRepository.ReadOutcomingCollectionAsync(post.BoardId, post.PostId, dbConnection, null);
                var incomingPostMentionCollection = await _postMentionRepository.ReadIncomingCollectionAsync(post.BoardId, post.PostId, dbConnection, null);

                post.OutcomingPostMentionCollection = outcomingPostMentionCollection;
                post.IncomingPostMentionCollection = incomingPostMentionCollection;
            }
        }

        public void Adapt(Board board, Post post)
        {
            var boardTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(board.TimeZone);

            post.Creation = DateTime.SpecifyKind(post.Creation, DateTimeKind.Utc);
            post.CreationLocal = TimeZoneInfo.ConvertTime(post.Creation, boardTimeZoneInfo);
            post.MessageDynamic = _markupService.MarkupDynamicCustomEncode(_markupService.MarkupDynamicEncode(post.BoardId, post.PostId, post.ThreadId, post.MessageStatic, post.OutcomingPostMentionCollection), board.MarkupDynamicCustomCollection);
        }
    }
}