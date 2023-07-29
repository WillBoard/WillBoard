using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.Translations
{
    public class TranslationsViewModel : AdministrationViewModel
    {
        public IEnumerable<Translation> TranslationCollection { get; set; }
    }
}