using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnlineConsulting.Api.Configurations.Extensions;

/// <summary>The "Bearer" requirement added by <c>AddSecurityRequirement</c> is document-level, so
/// Swagger UI shows a lock on every endpoint by default. Endpoints that never called
/// <c>RequireAuthorization()</c> get an explicit empty override here, which is how OpenAPI expresses
/// "no security" for one operation despite a document-wide default.</summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiresAuthorization = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Any();

        if (!requiresAuthorization)
        {
            operation.Security = [];
        }
    }
}
