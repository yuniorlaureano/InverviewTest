using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Data.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ISqlFactory, SqlFactory>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
