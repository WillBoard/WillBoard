namespace WillBoard.Core.Enums
{
    // Based on enum AVMediaType https://github.com/FFmpeg/FFmpeg/blob/master/libavutil/avutil.h
    public enum FFinformationStreamCodecType
    {
        Unknown,
        Video,
        Audio,
        Data,
        Subtitle,
        Attachment
    }
}