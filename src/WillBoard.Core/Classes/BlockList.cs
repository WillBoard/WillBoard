using System.Collections.Generic;

namespace WillBoard.Core.Classes
{
    public class BlockList
    {
        public string Address { get; set; }
        public IDictionary<string, string> HeaderDictionary { get; set; }
        public string[] ResponseCollection { get; set; }
    }
}