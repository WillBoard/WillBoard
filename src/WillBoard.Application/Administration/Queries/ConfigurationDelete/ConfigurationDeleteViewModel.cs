using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.ConfigurationDelete
{
    public class ConfigurationDeleteViewModel : AdministrationViewModel
    {
        public Configuration Configuration { get; set; }
    }
}