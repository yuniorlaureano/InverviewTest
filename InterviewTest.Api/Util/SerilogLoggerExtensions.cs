using Serilog;

namespace InterviewTest.Api.Util
{
    public static class SerilogLoggerExtensions
    {
        public static IHostBuilder AddSerilogAsLogger(this IHostBuilder host)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(GetConfiguration())
            .CreateLogger();


            host.UseSerilog();

            return host;
        }

        static IConfiguration GetConfiguration() =>
            new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
    }
}
