using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.TranslationDelete
{
    public class TranslationDeleteViewModel : AdministrationViewModel
    {
        public Translation Translation { get; set; }
    }
}