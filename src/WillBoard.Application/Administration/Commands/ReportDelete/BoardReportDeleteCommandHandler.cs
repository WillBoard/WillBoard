using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.ReportDelete
{
    public class ReportDeleteCommandHandler : IRequestHandler<ReportDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IReportRepository _reportRepository;
        private readonly IReportCache _reportCache;

        public ReportDeleteCommandHandler(AccountManager accountManager, IReportRepository reportRepository, IReportCache reportCache)
        {
            _accountManager = accountManager;
            _reportRepository = reportRepository;
            _reportCache = reportCache;
        }

        public async Task<Status<InternalError>> Handle(ReportDeleteCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var report = await _reportCache.GetSystemAsync(request.ReportId);

            if (report == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _reportRepository.DeleteAsync(report.ReportId);

            await _reportCache.RemoveSystemAsync(request.ReportId);
            await _reportCache.RemoveSystemIpCollectionAsync(report.IpVersion, report.IpNumber);
            await _reportCache.PurgeSystemCollectionAsync();
            await _reportCache.RemoveSystemCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}