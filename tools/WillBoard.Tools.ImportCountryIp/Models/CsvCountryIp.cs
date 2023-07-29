using CsvHelper.Configuration.Attributes;
using System;

namespace WillBoard.Tools.ImportCountryIp.Models
{
    public class CsvCountryIp
    {
        [Index(0)]
        public string From { get; set; }

        [Index(1)]
        public string To { get; set; }

        [Index(2)]
        public string Code { get; set; }
    }
}