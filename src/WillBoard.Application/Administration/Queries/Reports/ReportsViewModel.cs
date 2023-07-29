using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.Reports
{
    public class ReportsViewModel : AdministrationViewModel
    {
        public IDictionary<Report, Post> ReportDictionary { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}