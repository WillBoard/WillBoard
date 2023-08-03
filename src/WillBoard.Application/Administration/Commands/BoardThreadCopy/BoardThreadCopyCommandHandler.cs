using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Application.DataModels;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Services;

namespace WillBoard.Application.Administration.Commands.BoardThreadCopy
{
    public class BoardThreadCopyCommandHandler : IRequestHandler<BoardThreadCopyCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IPostService _postService;
        private readonly IStorageService _storageService;
        private readonly MarkupService _markupService;
        private readonly ISynchronizationService _synchronizationService;

        public BoardThreadCopyCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IBoardCache boardCache, IPostCache postCache, IPostService postService, IStorageService storageService, MarkupService markupService, ISynchronizationService synchronizationService)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _boardCache = boardCache;
            _postCache = postCache;
            _postService = postService;
            _storageService = storageService;
            _markupService = markupService;
            _synchronizationService = synchronizationService;
        }

        public async Task<Status<InternalError>> Handle(BoardThreadCopyCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionThreadCopy))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var thread = postCollection.FirstOrDefault(e => e.PostId == request.PostId);

            if (thread == null || thread.ThreadId != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var destinationBoard = await _boardCache.GetAsync(request.DestinationBoardId);

            if (destinationBoard == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(destinationBoard.BoardId, e => e.PermissionThreadCopy))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (board.BoardId == destinationBoard.BoardId)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var replyCollection = postCollection.Where(e => e.ThreadId == thread.PostId);

            var insertedPostIdCollection = new List<int>();

            var copyThread = new Post(thread);
            copyThread.BoardId = destinationBoard.BoardId;
            copyThread.Creation = _dateTimeProvider.UtcNow;
            copyThread.MessageStatic = _markupService.MarkupStaticCustomEncode(_markupService.MarkupStaticEncode(copyThread.MessageRaw), destinationBoard.MarkupStaticCustomCollection);

            var threadId = await _postService.CreateAsync(copyThread);
            insertedPostIdCollection.Add(threadId);

            _storageService.CopyPreviewFile(thread.BoardId, thread.FilePreviewName, copyThread.BoardId, copyThread.FilePreviewName);
            _storageService.CopySourceFile(thread.BoardId, thread.FileName, copyThread.BoardId, copyThread.FileName);

            foreach (var reply in replyCollection)
            {
                var copyReply = new Post(reply);
                copyReply.BoardId = destinationBoard.BoardId;
                copyReply.ThreadId = threadId;
                copyReply.Creation = _dateTimeProvider.UtcNow;
                copyReply.MessageStatic = _markupService.MarkupStaticCustomEncode(_markupService.MarkupStaticEncode(copyReply.MessageRaw), destinationBoard.MarkupStaticCustomCollection);

                var copyReplyId = await _postService.CreateAsync(copyReply);
                insertedPostIdCollection.Add(copyReplyId);

                _storageService.CopyPreviewFile(reply.BoardId, reply.FilePreviewName, copyReply.BoardId, copyReply.FilePreviewName);
                _storageService.CopySourceFile(reply.BoardId, reply.FileName, copyReply.BoardId, copyReply.FileName);
            }

            await _postCache.PurgeAdaptedCollectionAsync(destinationBoard.BoardId);

            foreach (var insertedPostId in insertedPostIdCollection)
            {
                var insertedPost = await _postCache.GetAdaptedAsync(destinationBoard, insertedPostId);
                if (insertedPost != null)
                {
                    var synchronizationMessage = new SynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Create,
                        Data = new PostDataModel(insertedPost, board, false)
                    };

                    var administrationSynchronizationMessage = new AdministrationSynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Create,
                        Data = new PostDataModel(insertedPost, board, administration: true)
                    };

                    _synchronizationService.Notify(synchronizationMessage, destinationBoard.BoardId);
                    _synchronizationService.Notify(administrationSynchronizationMessage, destinationBoard.BoardId);
                }
            }

            return Status<InternalError>.SuccessStatus();
        }
    }
}