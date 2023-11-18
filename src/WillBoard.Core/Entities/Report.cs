using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Report
    {
        public Guid ReportId { get; set; }

        public string BoardId { get; set; }

        public DateTime Creation { get; set; }

        public string ReferenceBoardId { get; set; }
        public int ReferencePostId { get; set; }

        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }

        public string Reason { get; set; }

        public Report()
        {
        }
    }
}