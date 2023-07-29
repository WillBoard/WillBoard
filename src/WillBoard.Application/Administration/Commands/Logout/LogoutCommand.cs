using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.Logout
{
    public class LogoutCommand : IRequest<Status<InternalError>>
    {
    }
}