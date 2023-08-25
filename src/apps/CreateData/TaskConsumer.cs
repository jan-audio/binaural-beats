using Serilog;
using System.Net.Http.Headers;

namespace CreateData
{
    public class TaskConsumerException : Exception
    {
        public TaskConsumerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TaskConsumerException ConnectException(string server, Exception ex)
        {
            return new TaskConsumerException($"Error in Connect to server {server}.", ex);
        }
    }

    internal class TaskConsumer : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly AppSettings _settings;

        public TaskConsumer(AppSettings settings)
        {
            _settings = settings;
            _client = new HttpClient();

            var uri = new Uri(_settings.ServerConnect);
            _client.Timeout = TimeSpan.FromSeconds(_settings.TimeOutConnect);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Worker");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("OSTime", DateTime.Now.ToLongTimeString());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
