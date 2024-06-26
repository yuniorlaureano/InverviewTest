﻿using Microsoft.Extensions.DependencyInjection;

namespace InterviewTest.Data.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ISqlFactory, SqlFactory>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IStockDetailRepository, StockDetailRepository>();
            services.AddScoped<IADOCommand, ADOCommand>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<ICityRepository, CityRepository>();

            return services;
        }
    }
}
