

using FluentMigrator.Runner;
using InterviewTest.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        var serviceProvider = CreateService();

        Console.WriteLine("");

        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider);
        }
    }

    static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(0);
        runner.MigrateUp(1234554);
    }

    static IServiceProvider CreateService()
    {
        var serviceCollection = new ServiceCollection();

        var configuration = GddConfigration();
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                rb.AddSqlServer();
                rb.WithGlobalConnectionString(configuration.GetConnectionString("InterviewTest"));
                rb.ScanIn(typeof(CreateTables).Assembly).For.Migrations();
            }).AddLogging(lb => lb.AddFluentMigratorConsole());

        return serviceCollection.BuildServiceProvider();
    }

    static IConfiguration GddConfigration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration;
    }

}



