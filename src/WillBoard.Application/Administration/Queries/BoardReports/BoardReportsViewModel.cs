using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardReports
{
    public class BoardReportsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
        public IDictionary<Report, Post> ReportDictionary { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}