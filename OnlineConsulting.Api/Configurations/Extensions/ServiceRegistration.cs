using Core.CrossCuttingConcernLayer.Loggings.Serilogs.Loggers;
using Core.CrossCuttingConcernLayer.Loggings.Serilogs.Services;
using Microsoft.OpenApi;
using OnlineConsulting.SharedKernel.DependencyInjection;
using System.Threading.RateLimiting;

namespace OnlineConsulting.Api.Configurations.Extensions;

public static class ServiceRegistration
{
    public const string AuthRateLimiterPolicy = "auth";
    public const string ReferralRedeemRateLimiterPolicy = "referral-redeem";

    public static void AddApiServiceRegistration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSharedKernel();

        // Identity/JWT wiring and role/permission policies live in IdentityModule and AuthorizationAddingBehavior, so this only needs "must be logged in".
        services.AddAuthorization();
        services.AddCors();
        services.AddSwagger();
        services.AddApiRateLimiting();

        // Backs ExceptionMiddleware (Program.cs), which logs every unhandled exception before mapping it to a ProblemDetails response.
        services.AddSingleton<BaseLoggerService, FileLogger>();
    }

    private static void AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 200,
                    Window = TimeSpan.FromMinutes(1),
                }));

            options.AddPolicy(AuthRateLimiterPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                }));

            options.AddPolicy(ReferralRedeemRateLimiterPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(httpContext), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                }));
        });
    }

    private static string GetPartitionKey(HttpContext httpContext) =>
        httpContext.User.Identity?.IsAuthenticated == true ? httpContext.User.Identity.Name ?? "anonymous" : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static void AddCors(this IServiceCollection services)
    {
        services.AddCors(opt => opt.AddDefaultPolicy(policy => policy
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineConsulting API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT access token (paste the raw token - \"Bearer \" is added automatically)",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };
            c.AddSecurityDefinition("Bearer", securityScheme);
            // Must bind the reference to the generated OpenApiDocument (hostDocument param), or Swashbuckle 10.x silently drops the requirement and the Authorize button stops attaching tokens.
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
            c.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}
