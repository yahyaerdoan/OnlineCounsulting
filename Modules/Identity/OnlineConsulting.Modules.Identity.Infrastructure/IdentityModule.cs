using Core.SecurityLayer.JsonWebTokens.Abstractions;
using Core.SecurityLayer.JsonWebTokens.Concretions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineConsulting.Modules.Identity.Application;
using OnlineConsulting.Modules.Identity.Application.Common.Templates;
using OnlineConsulting.Modules.Identity.Application.Features.Auth;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Abstractions;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Notifications;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.Modules.Identity.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Identity.Infrastructure.Repositories;
using OnlineConsulting.Modules.Identity.Infrastructure.Security;
using OnlineConsulting.Modules.Identity.Infrastructure.Seeding;
using OnlineConsulting.Modules.Identity.Infrastructure.Status;
using OnlineConsulting.Modules.Identity.Infrastructure.Storage;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using System.Text;

namespace OnlineConsulting.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    /// <summary>Host-agnostic wiring, used by both Api and UserInterface. Leaves auth scheme as cookie (Identity's default) - call <see cref="AddIdentityModuleJwtBearer"/> too for JWT bearer hosts.</summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<AppIdentityDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddIdentity<User, Role>(options => options.Password.RequiredLength = 6)
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<TokenOption>(configuration.GetSection("TokenOptions"));
        services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();

        services.AddScoped<ITokenService, TokenManager>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserImageStorage, UserImageStorage>();
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
        services.AddScoped<IInviteRepository, InviteRepository>();
        services.AddScoped<IUserExistenceReader, UserExistenceReader>();

        services.AddScoped<IEmailOutboxWriter, EmailOutboxWriter>();
        services.AddScoped<IEmailTemplate<ConfirmEmailEmailModel>, ConfirmEmailTemplate>();
        services.AddScoped<IEmailTemplate<WelcomeEmailModel>, WelcomeTemplate>();
        services.AddScoped<IEmailTemplate<PolicyNoticeEmailModel>, PolicyNoticeTemplate>();
        services.AddScoped<IEmailTemplate<InviteEmailModel>, InviteTemplate>();
        services.Configure<AuthEmailOptions>(configuration.GetSection("Auth"));
        services.Configure<SuperAdminSeedOptions>(configuration.GetSection("Seed:SuperAdmin"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdentityTransactionAddingBehavior<,>));

        return services;
    }

    public static IServiceCollection AddIdentityModuleJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenOption = configuration.GetSection("TokenOptions").Get<TokenOption>() ?? new TokenOption();

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
                ValidIssuer = tokenOption.Issuer,
                ValidAudience = tokenOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecurityKey)),
                ClockSkew = TimeSpan.Zero,
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                },
            };
        });

        return services;
    }
}
