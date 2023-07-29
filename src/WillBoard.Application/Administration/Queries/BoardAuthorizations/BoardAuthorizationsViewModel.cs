using System.Collections.Generic;
using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizations
{
    public class BoardAuthorizationsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
        public IEnumerable<WillBoard.Core.Entities.Authorization> AuthorizationCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}