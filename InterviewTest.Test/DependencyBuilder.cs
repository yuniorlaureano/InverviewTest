using InterviewTest.Data;
using InterviewTest.Data.Extensions;
using InterviewTest.Service.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewTest.Test
{
    internal class DependencyBuilder
    {
        public static IServiceProvider GetServices(Action<ServiceCollection>? func = null)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(GddConfigration());

            if (func == null)
            {
                services.AddRepositories();
                services.AddServices();
                services.AddMappings();
                services.AddValidators();
            }

            var mockLogger = new Mock<ILogger<ADOCommand>>();
            services.AddSingleton<ILogger<ADOCommand>>(mockLogger.Object);

            var mockHostEnv = new Mock<IHostEnvironment>();
            services.AddSingleton<IHostEnvironment>(mockHostEnv.Object);

            func?.Invoke(services);

            return services.BuildServiceProvider();
        }

        public static IConfiguration GddConfigration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var buildConfiguration = configurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            return buildConfiguration;
        }
    }
}
