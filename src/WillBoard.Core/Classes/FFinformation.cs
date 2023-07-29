using System.Collections.Generic;

namespace WillBoard.Core.Classes
{
    public class FFinformation
    {
        public FFinformationFormat Format { get; set; }
        public IEnumerable<FFinformationStream> StreamCollection { get; set; }
    }
}