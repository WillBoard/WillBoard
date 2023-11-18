using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardIpDeletePosts
{
    public class BoardIpDeletePostsCommandHandler : IRequestHandler<BoardIpDeletePostsCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IPostService _postService;
        private readonly IStorageService _storageService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly ILockManager _lockManager;

        public BoardIpDeletePostsCommandHandler(
            AccountManager accountManager,
            IBoardCache boardCache,
            IPostCache postCache,
            IPostService postService,
            IStorageService storageService,
            ISynchronizationService synchronizationService,
            ILockManager lockManager)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
            _postService = postService;
            _storageService = storageService;
            _synchronizationService = synchronizationService;
            _lockManager = lockManager;
        }

        public async Task<Status<InternalError>> Handle(BoardIpDeletePostsCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!UInt128.TryParse(request.IpNumber, out UInt128 ipNumber))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            if (ipNumber == 0)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionIpDeletePosts))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var postDeleteCollection = new List<Post>();

            using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
            {
                var postCollection = await _postCache.GetAdaptedCollectionAsync(board);

                var postDeleteThreadCollection = postCollection.Where(e => e.IpVersion == request.IpVersion && e.IpNumber == ipNumber && e.ThreadId == null);

                postDeleteCollection.AddRange(postDeleteThreadCollection);

                var postDeleteReplyCollection = postCollection.Where(e => e.IpVersion == request.IpVersion && e.IpNumber == ipNumber && e.ThreadId != null);

                postDeleteCollection.AddRange(postDeleteReplyCollection);

                foreach (var postDeleteThread in postDeleteThreadCollection)
                {
                    var replyCollection = postCollection.Where(e => e.ThreadId == postDeleteThread.PostId);
                    postDeleteCollection.AddRange(replyCollection);
                }

                await _postService.DeleteCollectionAsync(postDeleteCollection);

                foreach (var postDelete in postDeleteCollection)
                {
                    _storageService.DeleteSourceFile(board.BoardId, postDelete.FileName);
                    _storageService.DeletePreviewFile(board.BoardId, postDelete.FilePreviewName);

                    await _postCache.RemoveAdaptedAsync(board, postDelete.PostId);
                }
            }

            if (board.SynchronizationBoardAvailability || board.SynchronizationThreadAvailability)
            {
                foreach (var postDelete in postDeleteCollection)
                {
                    var synchronizationMessage = new SynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postDelete.BoardId,
                            postId = postDelete.PostId,
                            threadId = postDelete.ThreadId
                        }
                    };

                    var administrationSynchronizationMessage = new AdministrationSynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postDelete.BoardId,
                            postId = postDelete.PostId,
                            threadId = postDelete.ThreadId
                        }
                    };

                    _synchronizationService.Notify(synchronizationMessage, board.BoardId);
                    _synchronizationService.Notify(administrationSynchronizationMessage, board.BoardId);
                }
            }

            return Status<InternalError>.SuccessStatus();
        }
    }
}