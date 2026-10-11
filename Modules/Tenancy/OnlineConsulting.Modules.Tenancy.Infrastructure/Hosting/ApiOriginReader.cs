using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

public class ApiOriginReader(IOptions<TenantHostingOptions> options) : IApiOriginReader
{
    public string GetOrigin() => options.Value.ApiOrigin;
}
