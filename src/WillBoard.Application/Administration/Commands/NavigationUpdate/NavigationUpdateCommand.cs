using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.NavigationUpdate
{
    public class NavigationUpdateCommand : IRequest<Status<InternalError>>
    {
        public Guid NavigationId { get; set; }
        public int Priority { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool NewTab { get; set; }
    }
}