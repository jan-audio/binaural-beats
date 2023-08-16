using DotNetTools.SharpGrabber.Grabbed;
using Serilog;
using Serilog.Context;
using System.Collections.Concurrent;

namespace CreateData.YoutubeHelper
{
    public enum MediaDownloaderStatus
    {
        Pendding = 0x0,
        Running = 0x1,
        Done = 0x2,
        Exited = 0x3,
    }

    public class MediaDownloader : IDisposable
    {
        public MediaDownloaderStatus DownloadStatus { get; protected set; }

        private static readonly HttpClient _client = new HttpClient();
        private readonly string _targetPath;
        private readonly ILogger _logger;
        private readonly byte[] _buffer;

        public MediaDownloader(string targetPath, int bufferSize = 4096)
        {
            DownloadStatus = MediaDownloaderStatus.Pendding;
            _targetPath = targetPath;
            _buffer = new byte[bufferSize];

            LogContext.PushProperty("SourceContext", "MediaDownloader");
            _logger = Log.Logger;
        }

        #region Internal Methods

        private async Task<Stream> DownloadAsync(Uri uri, CancellationToken cancellation = default)
        {
            using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength;
            if (totalBytes.GetValueOrDefault() == 0)
                throw new Exception("No data to download.");
            return await response.Content.ReadAsStreamAsync(cancellation);
        }

        public async Task DownloadAsync(Uri uri, Stream outputStream, CancellationToken cancellation = default)
        {
            using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength;
            if (totalBytes.GetValueOrDefault() == 0)
                throw new Exception("No data to download.");
            using (var dowload = await response.Content.ReadAsStreamAsync(cancellation))
            {
                while (true)
                {
                    var countToRead = (int)Math.Min(totalBytes ?? int.MaxValue, _buffer.Length);
                    var read = await dowload.ReadAsync(_buffer, 0, countToRead, cancellation);
                    if (read <= 0 || cancellation.IsCancellationRequested)
                        break;
                    await outputStream.WriteAsync(_buffer, 0, read, cancellation);
                }
            }
        }

        #endregion Internal Methods

        #region Methods

        public async Task<List<Stream>> CompositeDownloadAsync(IEnumerable<GrabbedMedia> medias, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var total = medias.Count();
            var results = new List<Stream>(total);

            foreach (var (media, i) in medias.Select((media, i) => (media, i)))
            {
                try
                {
                    _logger.Information($"Download {media.Title}...");
                    results.Add(await DownloadAsync(media.ResourceUri, cancellationToken));
                    progress.Report(i / total);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Download error {media.Title}\n {ex}");
                }
            }
            return results;
        }

        public async Task<string[]> DownloadToFilesAsync(IEnumerable<GrabbedMedia> medias, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            var concu = new ConcurrentDictionary<int, string>();
            foreach (var (media, i) in medias.Select((media, i) => (media, i)))
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        var fileName = IOHelper.GenerateTempFile(_targetPath, media.Format.Extension);
                        using (var outputStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            _logger.Information($"Download {media.Title} To {_targetPath}/{Path.GetFileName(fileName)}...");
                            await DownloadAsync(media.ResourceUri, outputStream, cancellationToken);
                        }
                        concu.TryAdd(i, fileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Download error {media.Title}\n {ex}");
                        concu.TryAdd(i, string.Empty);
                    }
                });

                tasks.Add(task);
            }
            await Task.WhenAll(tasks.ToArray());

            return concu.OrderBy(o => o.Key).Select(s => s.Value).ToArray();
        }

        public void Dispose()
        {
            _client.Dispose();
            DownloadStatus = MediaDownloaderStatus.Exited;
        }

        #endregion Methods
    }
}
