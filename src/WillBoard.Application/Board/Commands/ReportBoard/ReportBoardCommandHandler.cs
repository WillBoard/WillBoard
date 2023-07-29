using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.ReportBoard
{
    public class ReportBoardCommandHandler : IRequestHandler<ReportBoardCommand, Status<InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReportRepository _reportRepository;
        private readonly IReportCache _reportCache;
        private readonly IPostCache _postCache;

        public ReportBoardCommandHandler(IpManager ipManager, BoardManager boardManager, IDateTimeProvider dateTimeProvider, IReportRepository reportRepository, IReportCache reportCache, IPostCache postCache)
        {
            _ipManager = ipManager;
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _reportRepository = reportRepository;
            _reportCache = reportCache;
            _postCache = postCache;
        }

        public async Task<Status<InternalError>> Handle(ReportBoardCommand request, CancellationToken cancellationToken)
        {
            if (request.Reason == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var board = _boardManager.GetBoard();

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (board.ReportBoardAvailability == false)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportBoardAvailability));
            }

            if (request.Reason.Length < board.ReportBoardLengthMin)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportBoardReasonLengthMin));
            }

            if (request.Reason.Length > board.ReportBoardLengthMax)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportBoardReasonLengthMax));
            }

            var reportCollection = await _reportCache.GetBoardIpCollectionAsync(board.BoardId, _ipManager.GetIpVersion(), _ipManager.GetIpNumber());

            if (board.ReportBoardIpMax < reportCollection.Count())
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportBoardIpMax));
            }

            var lastReport = reportCollection.OrderByDescending(r => r.Creation).FirstOrDefault();
            if (lastReport != null && lastReport.Creation.AddSeconds(board.ReportBoardTimeMin) > _dateTimeProvider.UtcNow)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportBoardTimeMin));
            }

            var report = new Report
            {
                ReportId = Guid.NewGuid(),
                BoardId = board.BoardId,
                Creation = _dateTimeProvider.UtcNow,
                ReferenceBoardId = board.BoardId,
                ReferencePostId = post.PostId,
                IpVersion = _ipManager.GetIpVersion(),
                IpNumber = _ipManager.GetIpNumber(),
                Reason = request.Reason
            };

            await _reportRepository.CreateAsync(report);

            await _reportCache.RemoveBoardIpCollectionAsync(board.BoardId, _ipManager.GetIpVersion(), _ipManager.GetIpNumber());
            await _reportCache.PurgeBoardCollectionAsync(board.BoardId);
            await _reportCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}