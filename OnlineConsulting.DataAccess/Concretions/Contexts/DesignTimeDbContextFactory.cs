using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.DataAccess.Concretions.Contexts;

/// <summary>Used only by `dotnet ef` at design time.</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OnlineConsultingDbContext>
{
    OnlineConsultingDbContext IDesignTimeDbContextFactory<OnlineConsultingDbContext>.CreateDbContext(string[] args)
        => new(DesignTimeDbContextOptionsFactory.Build<OnlineConsultingDbContext>(), new DummyHttpContextAccessor());
}

public class DummyHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = null;
}
