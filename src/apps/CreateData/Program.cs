// See https://aka.ms/new-console-template for more information
// https://learn.microsoft.com/en-us/dotnet/standard/commandline/handle-termination
using CreateData.YoutubeHelper;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.YouTube;
using NAudio.Wave;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace CreateData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Hello, World!");
            Console.WriteLine(DateTime.Now.ToFileTime());
            AppLogs.CreateLogger();
            if (File.Exists("Config.json"))
            {
                var json = File.ReadAllText("Config.json");
                var config = JsonSerializer.Deserialize<AppSettings>(json);
                Log.Logger.Information("loaded settings");
            }
            else
            {
                Log.Logger.Error("Thiếu config.");
                return;
            }

            // await DownloadNCT();create();
        }

        private static void create()
        {
            //var monoFilePath = Directory.GetFiles(IOHelper.GetTempPath("before")).FirstOrDefault();
            //using (var inputReader = new AudioFileReader(monoFilePath))
            //{
            //    // convert our mono ISampleProvider to stereo
            //    var panner = new PanningSampleProvider(inputReader);
            //    // override the default pan strategy
            //    panner.PanStrategy = new SquareRootPanStrategy();
            //    panner.Pan = -0.5f; // pan 50% left

            //    // can either use this for playback:
            //    //myOutputDevice.Init(panner);
            //    //myOutputDevice.Play();
            //    // ...

            //    // ... OR ... could write the stereo audio out to a WAV file
            //    var fileName = IOHelper.GenerateTempFile("after", "wav");
            //    WaveFileWriter.CreateWaveFile16(fileName, panner);
            //}
        }

        private static void ExtractChannels()
        {
            var file = Directory.GetFiles(IOHelper.GetTempPath("before")).FirstOrDefault();
            using (var reader = new Mp3FileReader(file))
            {
                Console.WriteLine("infor: {0}", reader.WaveFormat.ToString());

                var buffer = new byte[2 * reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                var format = new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, 1);
                var subFolder = Path.GetFileNameWithoutExtension(file);
                var path = IOHelper.GetTempPath(Path.Combine("after", subFolder ?? string.Empty));
                var channels = reader.WaveFormat.Channels;

                var writers = new WaveFileWriter[channels];
                for (int i = 0; i < channels; i++)
                {
                    var fileName = Path.Combine(path, String.Format("channel{0}.wav", i + 1));
                    writers[i] = new WaveFileWriter(fileName, format);
                }

                var st = Stopwatch.StartNew();

                //17802ms
                int bytesRead;
                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    int offset = 0;
                    while (offset < bytesRead)
                    {
                        for (int n = 0; n < writers.Length; n++)
                        {
                            // write one sample
                            writers[n].Write(buffer, offset, channels);
                            offset += channels;
                        }
                    }
                }
                Console.WriteLine($"{st.ElapsedMilliseconds}ms");

                for (int n = 0; n < writers.Length; n++)
                {
                    writers[n].Dispose();
                }
            }
        }

        private static async Task DownloadNCT()
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

        private static async Task DowloadYoutube()
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
