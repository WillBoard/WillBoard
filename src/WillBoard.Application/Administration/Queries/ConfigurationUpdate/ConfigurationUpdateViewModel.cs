using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.ConfigurationUpdate
{
    public class ConfigurationUpdateViewModel : AdministrationViewModel
    {
        public Configuration Configuration { get; set; }
    }
}