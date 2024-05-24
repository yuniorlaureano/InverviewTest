using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Common.Extensions
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddCommons(this IServiceCollection services)
        {
            services.AddSingleton<IExecutionResultFactory, ExecutionResultFactory>();
            return services;
        }
    }
}
