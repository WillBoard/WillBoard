using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Application.DataModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.AdministrationApi.Queries.GetPost
{
    public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Result<PostDataModel, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly AccountManager _accountManager;
        private readonly IPostCache _postCache;

        public GetPostQueryHandler(BoardManager boardManager, AccountManager accountManager, IPostCache postCache)
        {
            _boardManager = boardManager;
            _accountManager = accountManager;
            _postCache = postCache;
        }

        public async Task<Result<PostDataModel, InternalError>> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<PostDataModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var post = postCollection.FirstOrDefault(e => e.PostId == request.PostId);

            if (post == null)
            {
                return Result<PostDataModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var postDataModel = new PostDataModel(post, board, true);

            return Result<PostDataModel, InternalError>.ValueResult(postDataModel);
        }
    }
}