using System.Text;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PeladaPay.Application.Interfaces;
using PeladaPay.Application.Interfaces.BackgroundJobs;
using PeladaPay.Domain.Interfaces;
using PeladaPay.Infrastructure.Asaas;
using PeladaPay.Infrastructure.BackgroundJobs;
using PeladaPay.Infrastructure.Data;
using PeladaPay.Infrastructure.Payments;
using PeladaPay.Infrastructure.Persistence;
using PeladaPay.Infrastructure.Services;
using PeladaPay.Domain.Entities;
using Polly;
using System.Security.Claims;

namespace PeladaPay.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    private const string SecurityStampClaimType = "security_stamp";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var securityStamp = context.Principal?.FindFirstValue(SecurityStampClaimType);

                        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(securityStamp))
                        {
                            context.Fail("Token invalido.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user is null || !string.Equals(user.SecurityStamp, securityStamp, StringComparison.Ordinal))
                        {
                            context.Fail("Token expirado ou revogado.");
                        }
                    }
                };
            });

        services.AddAuthorization();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPaymentGatewayStrategy, AsaasPixGatewayStrategy>();

        services.AddOptions<AsaasOptions>()
            .Bind(configuration.GetSection(AsaasOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "Asaas:BaseUrl must be a valid absolute URL.")
            .ValidateOnStart();

        services.AddHttpClient<IAsaasService, AsaasService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<AsaasOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddResilienceHandler("asaas-http", resilience =>
            {
                resilience.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });
            });

        services.AddHangfire(config => config.UseMemoryStorage());
        services.AddHangfireServer();
        services.AddScoped<AsaasSubaccountRetryJob>();
        services.AddScoped<IAsaasSubaccountJobScheduler, AsaasSubaccountJobScheduler>();

        return services;
    }
}
