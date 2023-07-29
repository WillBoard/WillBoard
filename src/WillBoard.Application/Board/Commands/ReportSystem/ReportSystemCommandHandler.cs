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

namespace WillBoard.Application.Board.Commands.ReportSystem
{
    public class ReportSystemCommandHandler : IRequestHandler<ReportSystemCommand, Status<InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IConfigurationCache _configurationCache;
        private readonly IReportRepository _reportRepository;
        private readonly IReportCache _reportCache;
        private readonly IPostCache _postCache;

        public ReportSystemCommandHandler(IpManager ipManager, BoardManager boardManager, IDateTimeProvider dateTimeProvider, IConfigurationCache configurationCache, IReportRepository reportRepository, IReportCache reportCache, IPostCache postCache)
        {
            _ipManager = ipManager;
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _reportRepository = reportRepository;
            _configurationCache = configurationCache;
            _reportCache = reportCache;
            _postCache = postCache;
        }

        public async Task<Status<InternalError>> Handle(ReportSystemCommand request, CancellationToken cancellationToken)
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

            var reportSystem = await _configurationCache.GetAsync(ConfigurationKey.ReportSystemAvailability);
            if (reportSystem.Value == false)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportSystemAvailability));
            }

            var reportSystemLengthMin = await _configurationCache.GetAsync(ConfigurationKey.ReportSystemLengthMin);
            if (request.Reason.Length < reportSystemLengthMin.Value)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportSystemReasonLengthMin));
            }

            var reportSystemLengthMax = await _configurationCache.GetAsync(ConfigurationKey.ReportSystemLengthMax);
            if (request.Reason.Length > reportSystemLengthMax.Value)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportSystemReasonLengthMax));
            }

            var reportCollection = await _reportCache.GetSystemIpCollectionAsync(_ipManager.GetIpVersion(), _ipManager.GetIpNumber());

            var reportSystemIpMax = await _configurationCache.GetAsync(ConfigurationKey.ReportSystemIpMax);
            if (reportSystemIpMax.Value < reportCollection.Count())
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportSystemIpMax));
            }

            var lastReport = reportCollection.OrderByDescending(r => r.Creation).FirstOrDefault();
            var reportSystemTimeMin = await _configurationCache.GetAsync(ConfigurationKey.ReportSystemTimeMin);
            if (lastReport != null && lastReport.Creation.AddSeconds(reportSystemTimeMin.Value) > _dateTimeProvider.UtcNow)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReportSystemTimeMin));
            }

            var report = new Report
            {
                ReportId = Guid.NewGuid(),
                BoardId = null,
                Creation = _dateTimeProvider.UtcNow,
                ReferenceBoardId = board.BoardId,
                ReferencePostId = post.PostId,
                IpVersion = _ipManager.GetIpVersion(),
                IpNumber = _ipManager.GetIpNumber(),
                Reason = request.Reason
            };

            await _reportRepository.CreateAsync(report);

            await _reportCache.RemoveSystemIpCollectionAsync(_ipManager.GetIpVersion(), _ipManager.GetIpNumber());
            await _reportCache.PurgeSystemCollectionAsync();
            await _reportCache.RemoveSystemCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}