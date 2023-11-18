using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Services;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class PostCache : IPostCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IPostRepository _postRepository;
        private readonly IPostMentionRepository _postMentionRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;
        private readonly MarkupService _markupService;

        public PostCache(
            IMemoryCache memoryCache,
            IPostRepository postRepository,
            IPostMentionRepository postMentionRepository,
            ILockManager lockManager,
            ICancellationTokenManager cancellationTokenManager,
            MarkupService markupService)
        {
            _memoryCache = memoryCache;
            _postRepository = postRepository;
            _postMentionRepository = postMentionRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
            _markupService = markupService;
        }

        public async Task<Post> GetAdaptedAsync(Board board, int postId)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            return postCollection.FirstOrDefault(e => e.PostId == postId);
        }

        public async Task AddAdaptedAsync(Board board, Post post)
        {
            var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");

            using (await _lockManager.GetPostQueuedLockAsync($"Cache_UpdateAdapted_{board.BoardId}"))
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache currentPostMemoryCache))
                {
                    if (currentPostMemoryCache == null || currentPostMemoryCache.PostCollection == null)
                    {
                        return;
                    }

                    var currentPost = currentPostMemoryCache.PostCollection.FirstOrDefault(e => e.PostId == post.PostId);

                    if (currentPost != null)
                    {
                        return;
                    }

                    var postCollection = currentPostMemoryCache.PostCollection.ToList();

                    foreach (var outcomingPostMention in post.OutcomingPostMentionCollection)
                    {
                        var currentOutcomingPost = postCollection.FirstOrDefault(e => e.PostId == outcomingPostMention.IncomingPostId);

                        if (currentOutcomingPost != null)
                        {
                            var outcomingPost = new Post(currentOutcomingPost);
                            outcomingPost.IncomingPostMentionCollection = outcomingPost.IncomingPostMentionCollection.Append(outcomingPostMention);

                            postCollection.Remove(currentOutcomingPost);
                            postCollection.Add(outcomingPost);
                        }
                    }

                    foreach (var incomingPostMention in post.IncomingPostMentionCollection)
                    {
                        var currentIncomingPost = postCollection.FirstOrDefault(e => e.PostId == incomingPostMention.OutcomingPostId);

                        if (currentIncomingPost != null)
                        {
                            var incomingPost = new Post(currentIncomingPost);

                            foreach (var outcomingPostMention in incomingPost.OutcomingPostMentionCollection)
                            {
                                var postMention = new PostMention()
                                {
                                    OutcomingBoardId = incomingPost.BoardId,
                                    OutcomingPostId = incomingPost.PostId,
                                    OutcomingThreadId = incomingPost.ThreadId,
                                    IncomingBoardId = post.BoardId,
                                    IncomingPostId = post.PostId,
                                    IncomingThreadId = post.ThreadId,
                                    Active = true
                                };

                                incomingPost.OutcomingPostMentionCollection = incomingPost.OutcomingPostMentionCollection.Append(postMention);
                            }

                            incomingPost.MessageDynamic = _markupService.MarkupDynamicCustomEncode(_markupService.MarkupDynamicEncode(incomingPost.BoardId, incomingPost.PostId, incomingPost.ThreadId, incomingPost.MessageStatic, incomingPost.OutcomingPostMentionCollection), board.MarkupDynamicCustomCollection);

                            postCollection.Remove(currentIncomingPost);
                            postCollection.Add(incomingPost);
                        }
                    }

                    postCollection.Add(post);

                    if (post.ThreadId != null)
                    {
                        var currentThread = postCollection.FirstOrDefault(e => e.PostId == post.ThreadId);

                        if (currentThread != null)
                        {
                            var thread = new Post(currentThread);

                            thread.ReplyCount = thread.ReplyCount + 1;

                            if (post.Sage == false)
                            {
                                thread.Bump = post.Creation;
                            }

                            postCollection.Remove(currentThread);
                            postCollection.Add(thread);
                        }
                    }

                    var postMemoryCache = new PostMemoryCache()
                    {
                        PostCollection = postCollection,
                        AbsoluteExpiration = currentPostMemoryCache.AbsoluteExpiration
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = postMemoryCache.AbsoluteExpiration
                    };

                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);
                }
            }
        }

        public async Task UpdateAdaptedAsync(Board board, Post post)
        {
            var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");

            using (await _lockManager.GetPostQueuedLockAsync($"Cache_UpdateAdapted_{board.BoardId}"))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache currentPostMemoryCache))
                {
                    if (currentPostMemoryCache == null || currentPostMemoryCache.PostCollection == null)
                    {
                        return;
                    }

                    var currentPost = currentPostMemoryCache.PostCollection.FirstOrDefault(e => e.PostId == post.PostId);
                    if (currentPost == null)
                    {
                        return;
                    }

                    var postCollection = currentPostMemoryCache.PostCollection.ToList();

                    foreach (var outcomingPostMention in currentPost.OutcomingPostMentionCollection)
                    {
                        var currentOutcomingPost = postCollection.FirstOrDefault(e => e.PostId == outcomingPostMention.IncomingPostId);

                        if (currentOutcomingPost != null)
                        {
                            var outcomingPost = new Post(currentOutcomingPost);

                            var incomingPostMentionCollection = new List<PostMention>();

                            foreach (var currentIncomingPostMention in currentOutcomingPost.IncomingPostMentionCollection)
                            {
                                if (!(currentIncomingPostMention.OutcomingBoardId == currentPost.BoardId || currentIncomingPostMention.OutcomingPostId == currentPost.PostId))
                                {
                                    incomingPostMentionCollection.Add(currentIncomingPostMention);
                                }
                            }

                            outcomingPost.IncomingPostMentionCollection = incomingPostMentionCollection;

                            postCollection.Remove(currentOutcomingPost);
                            postCollection.Add(outcomingPost);
                        }
                    }

                    foreach (var outcomingPostMention in post.OutcomingPostMentionCollection)
                    {
                        var currentOutcomingPost = postCollection.FirstOrDefault(e => e.PostId == outcomingPostMention.IncomingPostId);

                        if (currentOutcomingPost != null)
                        {
                            var outcomingPost = new Post(currentOutcomingPost);

                            outcomingPost.IncomingPostMentionCollection = outcomingPost.IncomingPostMentionCollection.Append(outcomingPostMention);

                            postCollection.Remove(currentOutcomingPost);
                            postCollection.Add(outcomingPost);
                        }
                    }

                    postCollection.Remove(currentPost);
                    postCollection.Add(post);

                    var postMemoryCache = new PostMemoryCache()
                    {
                        PostCollection = postCollection,
                        AbsoluteExpiration = currentPostMemoryCache.AbsoluteExpiration
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = postMemoryCache.AbsoluteExpiration
                    };

                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);
                }
            }
        }

        public async Task UpdateAdaptedExcessiveAsync(Board board, int postId, DateTime? excessive)
        {
            var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");

            using (await _lockManager.GetPostQueuedLockAsync($"Cache_UpdateAdapted_{board.BoardId}"))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache currentPostMemoryCache))
                {
                    if (currentPostMemoryCache == null || currentPostMemoryCache.PostCollection == null)
                    {
                        return;
                    }

                    var currentPost = currentPostMemoryCache.PostCollection.FirstOrDefault(e => e.PostId == postId);
                    if (currentPost == null)
                    {
                        return;
                    }

                    var postCollection = currentPostMemoryCache.PostCollection.ToList();

                    var post = new Post(currentPost);
                    post.Excessive = excessive;

                    postCollection.Remove(currentPost);
                    postCollection.Add(post);

                    var postMemoryCache = new PostMemoryCache()
                    {
                        PostCollection = postCollection,
                        AbsoluteExpiration = currentPostMemoryCache.AbsoluteExpiration
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = postMemoryCache.AbsoluteExpiration
                    };

                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);
                }
            }
        }

        public async Task UpdateAdaptedFileDeletedAsync(Board board, int postId, bool fileDeleted)
        {
            var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");

            using (await _lockManager.GetPostQueuedLockAsync($"Cache_UpdateAdapted_{board.BoardId}"))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache currentPostMemoryCache))
                {
                    if (currentPostMemoryCache == null || currentPostMemoryCache.PostCollection == null)
                    {
                        return;
                    }

                    var currentPost = currentPostMemoryCache.PostCollection.FirstOrDefault(e => e.PostId == postId);
                    if (currentPost == null)
                    {
                        return;
                    }

                    var postCollection = currentPostMemoryCache.PostCollection.ToList();

                    var post = new Post(currentPost);
                    post.FileDeleted = fileDeleted;

                    postCollection.Remove(currentPost);
                    postCollection.Add(post);

                    var postMemoryCache = new PostMemoryCache()
                    {
                        PostCollection = postCollection,
                        AbsoluteExpiration = currentPostMemoryCache.AbsoluteExpiration
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = postMemoryCache.AbsoluteExpiration
                    };

                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);
                }
            }
        }

        public async Task RemoveAdaptedAsync(Board board, int postId)
        {
            var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");

            using (await _lockManager.GetPostQueuedLockAsync($"Cache_UpdateAdapted_{board.BoardId}"))
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache currentPostMemoryCache))
                {
                    if (currentPostMemoryCache == null || currentPostMemoryCache.PostCollection == null)
                    {
                        return;
                    }

                    var currentPost = currentPostMemoryCache.PostCollection.FirstOrDefault(e => e.PostId == postId);
                    if (currentPost == null)
                    {
                        return;
                    }

                    var postCollection = currentPostMemoryCache.PostCollection.ToList();

                    postCollection.Remove(currentPost);

                    foreach (var outcomingPostMention in currentPost.OutcomingPostMentionCollection)
                    {
                        var currentOutcomingPost = postCollection.FirstOrDefault(e => e.PostId == outcomingPostMention.IncomingPostId);

                        if (currentOutcomingPost != null)
                        {
                            var outcomingPost = new Post(currentOutcomingPost);

                            var incomingPostMentionCollection = new List<PostMention>();

                            foreach (var currentIncomingPostMention in currentOutcomingPost.IncomingPostMentionCollection)
                            {
                                if (!(currentIncomingPostMention.OutcomingBoardId == currentPost.BoardId && currentIncomingPostMention.OutcomingPostId == currentPost.PostId))
                                {
                                    incomingPostMentionCollection.Add(currentIncomingPostMention);
                                }
                            }

                            outcomingPost.IncomingPostMentionCollection = incomingPostMentionCollection;

                            postCollection.Remove(currentOutcomingPost);
                            postCollection.Add(outcomingPost);
                        }
                    }

                    foreach (var incomingPostMention in currentPost.IncomingPostMentionCollection)
                    {
                        var currentIncomingPost = postCollection.FirstOrDefault(e => e.PostId == incomingPostMention.OutcomingPostId);

                        if (currentIncomingPost != null)
                        {
                            var incomingPost = new Post(currentIncomingPost);

                            var outcomingPostMentionCollection = new List<PostMention>();

                            foreach (var currentOutcomingPostMention in currentIncomingPost.OutcomingPostMentionCollection)
                            {
                                if (!(currentOutcomingPostMention.IncomingBoardId == currentPost.BoardId && currentOutcomingPostMention.IncomingPostId == currentPost.PostId))
                                {
                                    outcomingPostMentionCollection.Add(currentOutcomingPostMention);
                                }
                            }

                            incomingPost.OutcomingPostMentionCollection = outcomingPostMentionCollection;
                            incomingPost.MessageDynamic = _markupService.MarkupDynamicCustomEncode(_markupService.MarkupDynamicEncode(incomingPost.BoardId, incomingPost.PostId, incomingPost.ThreadId, incomingPost.MessageStatic, incomingPost.OutcomingPostMentionCollection), board.MarkupDynamicCustomCollection);

                            postCollection.Remove(currentIncomingPost);
                            postCollection.Add(incomingPost);
                        }
                    }

                    if (currentPost.ThreadId != null)
                    {
                        var currentThread = postCollection.FirstOrDefault(e => e.PostId == currentPost.ThreadId);

                        if (currentThread != null)
                        {
                            var thread = new Post(currentThread);

                            thread.ReplyCount = thread.ReplyCount - 1;
                            var lastReply = postCollection.OrderByDescending(p => p.Creation).FirstOrDefault(e => e.ThreadId == currentPost.ThreadId && e.Sage == false);
                            if (lastReply == null)
                            {
                                thread.Bump = thread.Creation;
                            }
                            else
                            {
                                thread.Bump = lastReply.Creation;
                            }

                            postCollection.Remove(currentThread);
                            postCollection.Add(thread);
                        }
                    }

                    var postMemoryCache = new PostMemoryCache()
                    {
                        PostCollection = postCollection,
                        AbsoluteExpiration = currentPostMemoryCache.AbsoluteExpiration
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = postMemoryCache.AbsoluteExpiration
                    };

                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);
                }
            }
        }

        public async Task<IEnumerable<Post>> GetAdaptedCollectionAsync(Board board)
        {
            if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out PostMemoryCache postMemoryCache))
            {
                return postMemoryCache.PostCollection;
            }
            else
            {
                using (await _lockManager.GetPostLockAsync($"Cache_GetCollection_{board.BoardId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Post)}_Collection_{board.BoardId}", out postMemoryCache))
                    {
                        return postMemoryCache.PostCollection;
                    }

                    var postCollection = await _postRepository.ReadCollectionAsync(board.BoardId);

                    var outcomingPostMentionCollection = await _postMentionRepository.ReadOutcomingCollectionAsync(board.BoardId);
                    var incomingPostMentionCollection = await _postMentionRepository.ReadIncomingCollectionAsync(board.BoardId);

                    var outcomingPostMentionLookup = outcomingPostMentionCollection.ToLookup(e => e.OutcomingBoardId + "_" + e.OutcomingPostId);
                    var incomingPostMentionLookup = incomingPostMentionCollection.ToLookup(e => e.IncomingBoardId + "_" + e.IncomingPostId);

                    var boardTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(board.TimeZone);
                    foreach (var post in postCollection)
                    {
                        var postOutcomingPostMentionCollection = outcomingPostMentionLookup[post.BoardId + "_" + post.PostId];
                        if (postOutcomingPostMentionCollection != null)
                        {
                            post.OutcomingPostMentionCollection = postOutcomingPostMentionCollection;
                        }

                        var postIncomingPostMentionCollection = incomingPostMentionLookup[post.BoardId + "_" + post.PostId];
                        if (postIncomingPostMentionCollection != null)
                        {
                            post.IncomingPostMentionCollection = postIncomingPostMentionCollection;
                        }

                        post.Creation = DateTime.SpecifyKind(post.Creation, DateTimeKind.Utc);
                        post.CreationLocal = TimeZoneInfo.ConvertTime(post.Creation, boardTimeZoneInfo);
                        post.MessageDynamic = _markupService.MarkupDynamicCustomEncode(_markupService.MarkupDynamicEncode(post.BoardId, post.PostId, post.ThreadId, post.MessageStatic, post.OutcomingPostMentionCollection), board.MarkupDynamicCustomCollection);
                    }

                    var absoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60);

                    postMemoryCache = new PostMemoryCache()
                    {
                        AbsoluteExpiration = absoluteExpiration,
                        PostCollection = postCollection
                    };

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = absoluteExpiration
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetPostCancellationTokenSource($"Cache_GetCollection_{board.BoardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Post)}_Collection_{board.BoardId}", postMemoryCache, memoryCacheEntryOptions);

                    return postMemoryCache.PostCollection;
                }
            }
        }

        public async Task PurgeAdaptedCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemovePostCancellationTokenSource($"Cache_GetCollection_{boardId}");
        }

        public async Task<KeyValuePair<Post, Post[]>> GetAdaptedBoardThreadAsync(Board board, int threadId, int? last = null)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            var thread = postCollection.FirstOrDefault(p => p.PostId == threadId);

            if (thread == null || !thread.IsThread())
            {
                return new KeyValuePair<Post, Post[]>(null, null);
            }

            if (last == null || last < 50)
            {
                var replyCollection = postCollection.Where(p => p.ThreadId == threadId).OrderBy(p => p.Creation).ToArray();
                return new KeyValuePair<Post, Post[]>(thread, replyCollection);
            }

            var lastReplyCollection = postCollection.Where(p => p.ThreadId == threadId).OrderBy(p => p.Creation).TakeLast(last.Value).ToArray();
            return new KeyValuePair<Post, Post[]>(thread, lastReplyCollection);
        }

        public async Task<IDictionary<Post, Post[]>> GetAdaptedBoardAsync(Board board, int page)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            var threadList = postCollection.Where(p => p.ThreadId == null && p.Pin == false).OrderByDescending(p => p.Bump).Skip((page - 1) * board.PageThreadMax).Take(board.PageThreadMax).ToList();

            if (page == 1)
            {
                threadList.InsertRange(0, postCollection.Where(p => p.ThreadId == null && p.Pin == true).OrderByDescending(p => p.Bump));
            }

            var postDictionary = new Dictionary<Post, Post[]>();

            foreach (var thread in threadList)
            {
                postDictionary.Add(thread, postCollection.Where(p => thread.PostId == p.ThreadId && p.ThreadId != 0).OrderBy(p => p.Creation).TakeLast(thread.Pin ? board.ThreadPinReplyPreviewMax : board.ThreadReplyPreviewMax).ToArray());
            }

            return postDictionary;
        }

        public async Task<Post[]> GetAdaptedSearchAsync(Board board, int? postId, int? threadId, string message, string file, string type, string order)
        {
            if (string.IsNullOrEmpty(message))
            {
                if (postId is null && threadId is null)
                {
                    return Array.Empty<Post>();
                }
            }
            else
            {
                if (message.Length < 6 || message.Length > 32)
                {
                    return Array.Empty<Post>();
                }
            }

            var postCollection = await GetAdaptedCollectionAsync(board);

            if (!string.IsNullOrEmpty(message))
            {
                postCollection = postCollection.Where(e => e.MessageRaw.Contains(message, StringComparison.InvariantCultureIgnoreCase));
            }

            if (postId is not null)
            {
                postCollection = postCollection.Where(e => e.PostId == postId);
            }

            if (threadId is not null)
            {
                postCollection = postCollection.Where(e => e.ThreadId == threadId || (e.ThreadId == null && e.PostId == threadId));
            }

            if (file == "with")
            {
                postCollection = postCollection.Where(p => p.File);
            }

            if (file == "without")
            {
                postCollection = postCollection.Where(p => !p.File);
            }

            if (type == "thread")
            {
                postCollection = postCollection.Where(p => p.ThreadId == null);
            }

            if (type == "reply")
            {
                postCollection = postCollection.Where(p => p.ThreadId != null);
            }

            if (order == "desc")
            {
                postCollection = postCollection.OrderByDescending(e => e.PostId);
            }
            else
            {
                postCollection = postCollection.OrderBy(e => e.PostId);
            }

            return postCollection.ToArray();
        }

        public async Task<int> GetAdaptedBoardPageMaxAsync(Board board)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            var threadCount = postCollection.Where(p => p.ThreadId == null && p.Pin == false).Count();

            if (threadCount == 0)
            {
                return 1;
            }

            return ((threadCount - 1) / board.PageThreadMax) + 1;
        }

        public async Task<Post> GetAdaptedThreadLastByIpNumberAsync(Board board, IpVersion ipVersion, UInt128 ipNumber)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            return postCollection.Where(e => e.ThreadId == null && e.IpVersion == ipVersion && e.IpNumber == ipNumber).OrderByDescending(item => item.Creation).FirstOrDefault();
        }

        public async Task<Post> GetAdaptedReplyLastByIpNumberAsync(Board board, IpVersion ipVersion, UInt128 ipNumber)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            return postCollection.Where(e => e.ThreadId != null && e.IpVersion == ipVersion && e.IpNumber == ipNumber).OrderByDescending(item => item.Creation).FirstOrDefault();
        }

        public async Task<Post> GetAdaptedReplyUserIdAsync(Board board, int threadId, IpVersion ipVersion, UInt128 ipNumber)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            return postCollection.FirstOrDefault(e => (e.ThreadId == threadId || e.PostId == threadId) && e.IpVersion == ipVersion && e.IpNumber == ipNumber);
        }

        public async Task<Post> GetAdaptedByFileHashAsync(Board board, byte[] fileHash)
        {
            var postCollection = await GetAdaptedCollectionAsync(board);

            return postCollection.FirstOrDefault(e => e.FileHash == fileHash && e.FileDeleted == false);
        }
    }

    public class PostMemoryCache
    {
        public DateTimeOffset AbsoluteExpiration { get; set; }
        public IEnumerable<Post> PostCollection { get; set; }
    }
}