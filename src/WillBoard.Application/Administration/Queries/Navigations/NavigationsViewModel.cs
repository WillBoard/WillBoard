using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.Navigations
{
    public class NavigationsViewModel : AdministrationViewModel
    {
        public IEnumerable<Navigation> NavigationCollection { get; set; }
    }
}