using MediatR;
using Microsoft.AspNetCore.Http;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<Result<CreatePostDataModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int? ThreadId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public IFormFile File { get; set; }
        public bool OptionSpoiler { get; set; }
        public bool OptionSage { get; set; }
        public bool OptionUserId { get; set; }
        public bool OptionCountry { get; set; }
        public string UserAgent { get; set; }
        public string VerificationKey { get; set; }
        public string VerificationValue { get; set; }

        public CreatePostCommand()
        {
            BoardId = "";
            ThreadId = null;
            Name = "";
            Email = "";
            Subject = "";
            Message = "";
            Password = "";
            File = null;
            OptionSpoiler = false;
            OptionSage = false;
            OptionUserId = false;
            OptionCountry = false;
            UserAgent = "";
            VerificationKey = "";
            VerificationValue = "";
        }
    }
}