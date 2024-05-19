using InterviewTest.Data.Extensions;
using InterviewTest.Service.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
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

builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddMappings();
builder.Services.AddValidators();
builder.Services.AddJwt(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
