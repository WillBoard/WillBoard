using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Search
{
    public class SearchQueryHandler : IRequestHandler<SearchQuery, Result<SearchViewModel, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;

        public SearchQueryHandler(BoardManager boardManager, IPostCache postCache)
        {
            _boardManager = boardManager;
            _postCache = postCache;
        }

        public async Task<Result<SearchViewModel, InternalError>> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!board.SearchAvailability)
            {
                return Result<SearchViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var postCollection = await _postCache.GetAdaptedSearchAsync(board, request.PostId, request.ThreadId, request.Message, request.File, request.Type, request.Order);

            var result = new SearchViewModel()
            {
                BoardViewType = BoardViewType.Search,
                Title = $"/{board.BoardId}/ - {board.Name}",
                SearchCollection = postCollection,
                PostId = request.PostId,
                ThreadId = request.ThreadId,
                Message = request.Message,
                File = request.File,
                Type = request.Type,
                Order = request.Order,
            };

            return Result<SearchViewModel, InternalError>.ValueResult(result);
        }
    }
}