using MediatR;
using System;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.Login
{
    public class LoginCommand : IRequest<Result<Authentication, InternalError>>
    {
        public Guid AccountId { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string VerificationKey { get; set; }
        public string VerificationValue { get; set; }
    }
}