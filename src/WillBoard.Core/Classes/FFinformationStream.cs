using WillBoard.Core.Enums;

namespace WillBoard.Core.Classes
{
    public class FFinformationStream
    {
        public int Index { get; set; }
        public string CodecName { get; set; }
        public FFinformationStreamCodecType CodecType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}