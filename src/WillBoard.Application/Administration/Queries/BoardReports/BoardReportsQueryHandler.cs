using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardReports
{
    public class BoardReportsQueryHandler : IRequestHandler<BoardReportsQuery, Result<BoardReportsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IReportCache _reportCache;
        private readonly IPostCache _postCache;

        public BoardReportsQueryHandler(AccountManager accountManager, IBoardCache boardCache, IReportCache reportCache, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _reportCache = reportCache;
            _postCache = postCache;
        }

        public async Task<Result<BoardReportsViewModel, InternalError>> Handle(BoardReportsQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardReportsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionReportRead))
            {
                return Result<BoardReportsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var reportCount = await _reportCache.GetBoardCountAsync(board.BoardId);
            var pageMax = (reportCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardReportsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var reportCollection = await _reportCache.GetBoardCollectionAsync(board.BoardId, (request.Page - 1) * 100, 100);

            var reportDictionary = new Dictionary<Report, Post>();
            foreach (var report in reportCollection)
            {
                var post = await _postCache.GetAdaptedAsync(board, report.ReferencePostId);
                reportDictionary.Add(report, post);
            }

            var result = new BoardReportsViewModel()
            {
                Board = board,
                ReportDictionary = reportDictionary,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardReportsViewModel, InternalError>.ValueResult(result);
        }
    }
}