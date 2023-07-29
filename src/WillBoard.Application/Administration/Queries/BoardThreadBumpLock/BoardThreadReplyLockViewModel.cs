using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardThreadReplyLock
{
    public class BoardThreadReplyLockViewModel : AdministrationViewModel
    {
        public Post Post { get; set; }
    }
}