using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace InterviewTest.Api.Util
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "InterviewTest Api",
                    Version = "v1"
                });

                var security = new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                x.AddSecurityDefinition(security.Reference.Id, security);
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {security, Array.Empty<string>() }
                });
            });

            return services;
        }
    }
}
