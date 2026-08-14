using Core.CrossCuttingConcernLayer.Loggings.Serilogs.Loggers;
using Core.CrossCuttingConcernLayer.Loggings.Serilogs.Services;
using Microsoft.OpenApi;
using OnlineConsulting.SharedKernel.DependencyInjection;

namespace OnlineConsulting.Api.Configurations.Extensions;

public static class ServiceRegistration
{
    public static void AddApiServiceRegistration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOnlineConsultingConventionServices();

        // Identity/JWT bearer wiring lives in IdentityModule.AddIdentityModule() /
        // AddIdentityModuleJwtBearer() (Program.cs) - Auth is a module like Categories, not
        // registered inline here. Role/permission checks moved into AuthorizationAddingBehavior
        // (per-command Roles arrays), so there's no policy to register here anymore - endpoints
        // only call .RequireAuthorization() for "must be logged in."
        services.AddAuthorization();
        services.AddCors();
        services.AddSwagger();

        // Backs ExceptionMiddleware (Program.cs), which logs every unhandled exception before
        // mapping it to a ProblemDetails response.
        services.AddSingleton<BaseLoggerService, FileLogger>();
    }

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
            // AddSecurityRequirement's delegate receives the OpenApiDocument being generated -
            // the reference must be bound to that document (hostDocument param), or Swashbuckle
            // 10.x silently drops the requirement and no "security" ends up in swagger.json (root
            // or per-operation), so the Authorize button never actually attaches the token to
            // requests.
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
            c.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}
