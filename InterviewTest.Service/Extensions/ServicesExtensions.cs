using FluentValidation;
using InterviewTest.Common;
using InterviewTest.Common.Dto;
using InterviewTest.Service.Interfaces;
using InterviewTest.Service.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InterviewTest.Service.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<ICityService, CityService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<UserCreationDto>, UserCreationValidator>();
            services.AddScoped<IValidator<UserUpdateDto>, UserUpdateValidator>();

            services.AddScoped<IValidator<UserLoginDto>, UserLoginValidator>();

            services.AddScoped<IValidator<ProductCreationDto>, ProductCreationValidator>();
            services.AddScoped<IValidator<ProductUpdateDto>, ProductUpdateValidator>();

            services.AddScoped<IValidator<StockCreationDto>, StockCreationValidator>();
            services.AddScoped<IValidator<StockUpdateDto>, StockUpdateValidator>();

            services.AddScoped<IValidator<CountryCreationDto>, CountryCreationValidator>();
            services.AddScoped<IValidator<CountryUpdateDto>, CountryUpdateValidator>();

            services.AddScoped<IValidator<ProvinceCreationDto>, ProvinceCreationValidator>();
            services.AddScoped<IValidator<ProvinceUpdateDto>, ProvinceUpdateValidator>();

            services.AddScoped<IValidator<CityCreationDto>, CityCreationValidator>();
            services.AddScoped<IValidator<CityUpdateDto>, CityUpdateValidator>();

            services.AddScoped<IJwtService, JwtService>();
            return services;
        }

        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtOptions = configuration.SetJwtOptions();

            services.AddSingleton(jwtOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddAuthorization();

            return services;
        }

        private static JwtOptions SetJwtOptions(this IConfiguration configuration)
        {
            var audience = configuration.GetValue<string>("JwtOptions:Audience");
            var issuer = configuration.GetValue<string>("JwtOptions:Issuer");
            var secret = configuration.GetValue<string>("JwtOptions:Secret");
            var expiresIn = configuration.GetValue<string>("JwtOptions:ExpiresIn");

            if (
                audience is string { Length: 0 } ||
                issuer is string { Length: 0 } ||
                secret is string { Length: 0 } ||
                expiresIn is string { Length: 0 })
            {
                throw new ArgumentNullException("Must provide Jwt options");
            }


            return new JwtOptions(secret, issuer, audience, int.Parse(expiresIn));
        }
    }
}
