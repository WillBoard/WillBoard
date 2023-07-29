using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.Configurations
{
    public class ConfigurationsViewModel : AdministrationViewModel
    {
        public IEnumerable<Configuration> ConfigurationCollection { get; set; }
    }
}