using DotNetTools.SharpGrabber.Converter;
using DotNetTools.SharpGrabber.Grabbed;

namespace CreateData.YoutubeHelper
{
    public static class ConvertHelper
    {
        private static readonly Dictionary<string, HashSet<string>> ContainerMimeSupport = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "mp4", new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "video/mp4", "audio/mp4" } },
            { "webm", new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "video/webm", "audio/webm" } }
        };

        public static void MergeVideoAudio(string videoPath, string audioPath, string outputPath, MediaFormat mediaFormat)
        {
            var builder = new MediaMerger(outputPath);
            builder.AddStreamSource(videoPath, MediaStreamType.Video);
            builder.AddStreamSource(audioPath, MediaStreamType.Audio);
            builder.OutputMimeType = mediaFormat.Mime;
            builder.OutputShortName = mediaFormat.Extension;
            builder.Build();
        }
    }
}
