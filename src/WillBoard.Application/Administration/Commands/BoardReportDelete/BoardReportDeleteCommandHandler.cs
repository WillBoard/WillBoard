using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardReportDelete
{
    public class BoardReportDeleteCommandHandler : IRequestHandler<BoardReportDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IReportRepository _reportRepository;
        private readonly IBoardCache _boardCache;
        private readonly IReportCache _reportCache;

        public BoardReportDeleteCommandHandler(AccountManager accountManager, IReportRepository reportRepository, IBoardCache boardCache, IReportCache reportCache)
        {
            _accountManager = accountManager;
            _reportRepository = reportRepository;
            _boardCache = boardCache;
            _reportCache = reportCache;
        }

        public async Task<Status<InternalError>> Handle(BoardReportDeleteCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionReportDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var report = await _reportCache.GetBoardAsync(board.BoardId, request.ReportId);

            if (report == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _reportRepository.DeleteAsync(report.ReportId);

            await _reportCache.RemoveBoardAsync(board.BoardId, request.ReportId);
            await _reportCache.RemoveBoardIpCollectionAsync(board.BoardId, report.IpVersion, report.IpNumber);
            await _reportCache.PurgeBoardCollectionAsync(board.BoardId);
            await _reportCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}