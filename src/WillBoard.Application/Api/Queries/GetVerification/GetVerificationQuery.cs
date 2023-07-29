using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Api.Queries.GetVerification
{
    public class GetVerificationQuery : IRequest<Result<bool, InternalError>>
    {
        public string BoardId { get; set; }
        public bool Thread { get; set; }
    }
}