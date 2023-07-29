using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Api.Queries.GetVerification
{
    public class GetVerificationQueryHandler : IRequestHandler<GetVerificationQuery, Result<bool, InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IpManager _ipManager;
        private readonly IVerificationService _verificationService;

        public GetVerificationQueryHandler(BoardManager boardManager, IpManager ipManager, IVerificationService verificationService)
        {
            _boardManager = boardManager;
            _ipManager = ipManager;
            _verificationService = verificationService;
        }

        public async Task<Result<bool, InternalError>> Handle(GetVerificationQuery request, CancellationToken cancellationToken)
        {
            var verification = await _verificationService.CheckAsync(request.Thread, _ipManager.GetIpVersion(), _ipManager.GetIpNumber(), _boardManager.GetBoard());

            return Result<bool, InternalError>.ValueResult(verification);
        }
    }
}