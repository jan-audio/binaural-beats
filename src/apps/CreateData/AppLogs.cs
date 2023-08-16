using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Runtime.CompilerServices;

namespace CreateData
{
    public static class AppLogs
    {
        public static void CreateLogger()
        {
            string filelog = "Apps.log";
            String logTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] [{SourceContext}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: logTemplate, theme: AnsiConsoleTheme.Sixteen)
                // .WriteTo.File(filelog, outputTemplate: logTemplate)
                .CreateLogger();
        }

        public static void Information<T>(this ILogger logger, string message)
        {
            //[{SourceContext}]
            logger.Information(message.FormatForContext(typeof(T).Name));
        }

        private static string FormatForContext(this string message, [CallerMemberName] string memberName = "")
        {
            if (string.IsNullOrWhiteSpace(memberName))
                return message;

            return $"[{memberName}] {message}";
        }
    }
}
