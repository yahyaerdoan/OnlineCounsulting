namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>A DbContext holding tenant-scoped entities. The tenant query filter reads CurrentTenantId from the context running the query;
/// a filter that captures anything else is evaluated once and then reused for every tenant.</summary>
public interface ITenantScopedDbContext
{
    Guid CurrentTenantId { get; }
}
