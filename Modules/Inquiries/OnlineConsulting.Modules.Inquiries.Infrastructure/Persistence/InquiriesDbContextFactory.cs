using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

/// <summary>Design-time only - the real ITenantProvider comes from the request scope at runtime.</summary>
public class InquiriesDbContextFactory : IDesignTimeDbContextFactory<InquiriesDbContext>
{
    public InquiriesDbContext CreateDbContext(string[] args) => new(DesignTimeDbContextOptionsFactory.Build<InquiriesDbContext>(), new NullTenantProvider());
}
