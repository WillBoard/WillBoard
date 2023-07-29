using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Reports
{
    public class ReportsQueryHandler : IRequestHandler<ReportsQuery, Result<ReportsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IReportCache _reportCache;

        public ReportsQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache, IReportCache reportCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
            _reportCache = reportCache;
        }

        public async Task<Result<ReportsViewModel, InternalError>> Handle(ReportsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<ReportsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var reportCount = await _reportCache.GetSystemCountAsync();
            var pageMax = (reportCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<ReportsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var reportCollection = await _reportCache.GetSystemCollectionAsync((request.Page - 1) * 100, 100);

            var reportDictionary = new Dictionary<Report, Post>();
            foreach (var report in reportCollection)
            {
                var board = await _boardCache.GetAsync(report.ReferenceBoardId);
                var post = await _postCache.GetAdaptedAsync(board, report.ReferencePostId);
                reportDictionary.Add(report, post);
            }

            var result = new ReportsViewModel()
            {
                ReportDictionary = reportDictionary,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<ReportsViewModel, InternalError>.ValueResult(result);
        }
    }
}