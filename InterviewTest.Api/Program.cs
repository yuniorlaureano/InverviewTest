using InterviewTest.Api.Util;
using InterviewTest.Common.Extensions;
using InterviewTest.Data.Extensions;
using InterviewTest.Service.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Host.AddSerilogAsLogger();

builder.Services.AddCommons();
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
