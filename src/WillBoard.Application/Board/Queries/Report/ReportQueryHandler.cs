using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Report
{
    public class ReportQueryHandler : IRequestHandler<ReportQuery, Result<PostViewModel, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;

        public ReportQueryHandler(BoardManager boardManager, IPostCache postCache)
        {
            _boardManager = boardManager;
            _postCache = postCache;
        }

        public async Task<Result<PostViewModel, InternalError>> Handle(ReportQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();
            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null)
            {
                return Result<PostViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new PostViewModel()
            {
                BoardViewType = BoardViewType.Other,
                Title = $"/{board.BoardId}/ - {board.Name}",
                Post = post
            };

            return Result<PostViewModel, InternalError>.ValueResult(result);
        }
    }
}