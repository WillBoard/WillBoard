using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.NavigationCreate
{
    public class NavigationCreateCommand : IRequest<Status<InternalError>>
    {
        public int Priority { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool NewTab { get; set; }
    }
}