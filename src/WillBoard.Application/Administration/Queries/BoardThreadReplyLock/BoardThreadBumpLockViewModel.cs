using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardThreadBumpLock
{
    public class BoardThreadBumpLockViewModel : AdministrationViewModel
    {
        public Post Post { get; set; }
    }
}