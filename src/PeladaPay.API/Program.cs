using Hangfire;
using PeladaPay.API.Configurations;
using PeladaPay.API.Middlewares;
using PeladaPay.API.Services;
using PeladaPay.Application.Common;
using PeladaPay.Application.Interfaces;
using PeladaPay.Infrastructure.DependencyInjection;
using PeladaPay.Infrastructure.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins(
                "https://peladapay.duckdns.org"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // só se usar cookie/token
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Production");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/hangfire");

await app.ApplyMigrationsAsync();

app.Run();
