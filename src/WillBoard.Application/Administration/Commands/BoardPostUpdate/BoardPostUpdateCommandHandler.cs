using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Services;

namespace WillBoard.Application.Administration.Commands.BoardPostUpdate
{
    public class BoardPostUpdateCommandHandler : IRequestHandler<BoardPostUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IPostRepository _postRepository;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly MarkupService _markupService;

        public BoardPostUpdateCommandHandler(AccountManager accountManager, IPostRepository postRepository, IBoardCache boardCache, IPostCache postCache, MarkupService markupService)
        {
            _accountManager = accountManager;
            _postRepository = postRepository;
            _boardCache = boardCache;
            _postCache = postCache;
            _markupService = markupService;
        }

        public async Task<Status<InternalError>> Handle(BoardPostUpdateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionPostEdit))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);
            if (post == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var updatePost = new Post(post);

            updatePost.Subject = request.Subject;
            updatePost.Name = request.Name;
            updatePost.Email = request.Email;
            updatePost.MessageRaw = request.Message;
            updatePost.MessageStatic = _markupService.MarkupStaticCustomEncode(_markupService.MarkupStaticEncode(request.Message), board.MarkupStaticCustomCollection);
            updatePost.MessageDynamic = _markupService.MarkupDynamicCustomEncode(_markupService.MarkupDynamicEncode(updatePost.BoardId, updatePost.PostId, updatePost.ThreadId, updatePost.MessageStatic, updatePost.OutcomingPostMentionCollection), board.MarkupDynamicCustomCollection);
            updatePost.Password = request.Password;

            await _postRepository.UpdateAsync(updatePost);

            await _postCache.PurgeAdaptedCollectionAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}