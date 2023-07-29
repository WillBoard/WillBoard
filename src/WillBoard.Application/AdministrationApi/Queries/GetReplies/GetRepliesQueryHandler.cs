using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Application.DataModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.AdministrationApi.Queries.GetReplies
{
    public class GetRepliesQueryHandler : IRequestHandler<GetRepliesQuery, Result<IEnumerable<PostDataModel>, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly AccountManager _accountManager;
        private readonly IPostCache _postCache;

        public GetRepliesQueryHandler(BoardManager boardManager, AccountManager accountManager, IPostCache postCache)
        {
            _boardManager = boardManager;
            _accountManager = accountManager;
            _postCache = postCache;
        }

        public async Task<Result<IEnumerable<PostDataModel>, InternalError>> Handle(GetRepliesQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<IEnumerable<PostDataModel>, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var thread = postCollection.FirstOrDefault(e => e.PostId == request.ThreadId);

            if (thread == null)
            {
                return Result<IEnumerable<PostDataModel>, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!thread.IsThread())
            {
                return Result<IEnumerable<PostDataModel>, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var replyCollection = postCollection.Where(p => p.ThreadId == request.ThreadId).OrderBy(p => p.Creation);

            if (request.Last != null)
            {
                var lastResult = replyCollection.TakeLast(request.Last.Value).Select(e => new PostDataModel(e, board, true));
                return Result<IEnumerable<PostDataModel>, InternalError>.ValueResult(lastResult);
            }

            var result = replyCollection.Select(e => new PostDataModel(e, board, true));
            return Result<IEnumerable<PostDataModel>, InternalError>.ValueResult(result);
        }
    }
}