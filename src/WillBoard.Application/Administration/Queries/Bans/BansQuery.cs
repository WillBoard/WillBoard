using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Bans
{
    public class BansQuery : IRequest<Result<BansViewModel, InternalError>>
    {
        public int Page { get; set; }
    }
}