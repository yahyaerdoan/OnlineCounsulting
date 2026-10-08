using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Api.Common;

/// <summary>Resolves the X-Tenant-Host header for TenantProvider; an unknown host gets 404 so a mistyped or retired site never falls back to the platform's data.</summary>
public class TenantHostMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantHostResolver resolver)
    {
        var host = context.Request.Headers[TenantHeaders.Host].ToString();

        if (string.IsNullOrWhiteSpace(host))
        {
            await next(context);
            return;
        }

        var tenantId = await resolver.ResolveAsync(host, context.RequestAborted);

        if (tenantId is null)
        {
            await Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Unknown site", detail: $"No business is set up for '{host}'.").ExecuteAsync(context);
            return;
        }

        context.Items[TenantProvider.HostTenantItemKey] = tenantId.Value;
        await next(context);
    }
}
