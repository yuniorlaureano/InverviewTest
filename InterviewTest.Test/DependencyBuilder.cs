using InterviewTest.Data.Extensions;
using InterviewTest.Service.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Test
{
    internal class DependencyBuilder
    {
        public static IServiceProvider GetServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(GddConfigration());


            services.AddRepositories();
            services.AddServices();
            services.AddMappings();
            services.AddValidators();


            return services.BuildServiceProvider();
        }

        public static IConfiguration GddConfigration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var buildConfiguration = configurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build(); ;

            return buildConfiguration;
        }
    }
}
