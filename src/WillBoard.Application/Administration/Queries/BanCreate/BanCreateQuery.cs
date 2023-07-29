using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanCreate
{
    public class BanCreateQuery : IRequest<Result<BanCreateViewModel, InternalError>>
    {
        public IpVersion IPVersion { get; set; }
        public string IPNumber { get; set; }
    }
}