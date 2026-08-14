using Core.SecurityLayer.JsonWebTokens.Abstractions;
using Core.SecurityLayer.JsonWebTokens.Concretions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.Modules.Identity.Infrastructure.Repositories;
using OnlineConsulting.Modules.Identity.Infrastructure.Security;
using OnlineConsulting.Modules.Identity.Infrastructure.Storage;
using System.Text;

namespace OnlineConsulting.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    /// <summary>Host-agnostic wiring, used by both Api and UserInterface. Leaves auth scheme as cookie (Identity's default) - call <see cref="AddIdentityModuleJwtBearer"/> too for JWT bearer hosts.</summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(connectionString));

        services.AddIdentity<User, Role>(options => options.Password.RequiredLength = 6)
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<TokenOption>(configuration.GetSection("TokenOptions"));
        services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();

        services.AddScoped<ITokenService, TokenManager>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserImageStorage, UserImageStorage>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ITokenService).Assembly));
        services.AddValidatorsFromAssembly(typeof(ITokenService).Assembly);

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
        });

        return services;
    }
}
