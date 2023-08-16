// See https://aka.ms/new-console-template for more information
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/handle-termination
using CreateData.YoutubeHelper;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.YouTube;
using Serilog;
using System.Text;

namespace CreateData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Hello, World!");
            var progressBar = new Progress<double>();
            AppLogs.CreateLogger();

            var link = "https://c1-ex-swe.nixcdn.com/NhacCuaTui2042/PhaiDauCuocTinhRemixTbynzRemix-DJ-9873699_hq.mp3?st=20NzwSd5MgTr4jAPxDU9Tw&e=1692783277&download=true";
            var uri = new Uri(link);
            var downloader = new MediaDownloader("before");
            var fileName = IOHelper.GenerateTempFile("before", "mp3");
            using (var outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Log.Logger.Information($"Download  To {fileName}...");
                await downloader.DownloadAsync(uri, outputStream);
            }
            Console.WriteLine(fileName);
        }

        private async Task DownloadNCT()
        {
            var link = "https://c1-ex-swe.nixcdn.com/NhacCuaTui2042/PhaiDauCuocTinhRemixTbynzRemix-DJ-9873699_hq.mp3?st=20NzwSd5MgTr4jAPxDU9Tw&e=1692783277&download=true";
            var uri = new Uri(link);
            var downloader = new MediaDownloader("before");
            var fileName = IOHelper.GenerateTempFile("before", "mp3");
            using (var outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Log.Logger.Information($"Download  To {fileName}...");
                await downloader.DownloadAsync(uri, outputStream);
            }
            Console.WriteLine(fileName);
        }

        private async Task DowloadYoutube()
        {
            var uri = new Uri("https://www.youtube.com/watch?v=HF4W4W5nQFc&t=116s");
            var grabber = new YouTubeGrabber(GrabberServices.Default);
            var result = await grabber.GrabAsync(uri);

            if (result == null)
                return;

            Console.WriteLine(result.Title);
            //var medias = result.Resources.Where(e => (e as GrabbedMedia) != null && ((e as GrabbedMedia).Channels == MediaChannels.Audio || (e as GrabbedMedia).Channels == MediaChannels.Both));
            var medias = result.Resources.OfType<GrabbedMedia>()// .Where(e => e is GrabbedMedia).Select(s => s as GrabbedMedia)
                .Where(e => e.Channels == MediaChannels.Audio || e.Channels == MediaChannels.Both).ToArray() ?? Array.Empty<GrabbedMedia>();

            var downloader = new MediaDownloader("before");
            var dowloadedFilesTask = await downloader.DownloadToFilesAsync(medias.Take(1));
        }
    }
}
