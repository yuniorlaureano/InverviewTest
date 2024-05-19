using InterviewTest.Service.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Service.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(UserMapping));
                config.AddMaps(typeof(ProductMapping));
                config.AddMaps(typeof(StockDetailMapping));
                config.AddMaps(typeof(StockMapping));
            });

            return services;
        }
    }
}
