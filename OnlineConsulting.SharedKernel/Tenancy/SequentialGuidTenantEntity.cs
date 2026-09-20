namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Guid-keyed TenantEntity that self-assigns a UUIDv7 (time-ordered) id - see SequentialGuidEntity.</summary>
public abstract class SequentialGuidTenantEntity : TenantEntity<Guid>
{
    protected SequentialGuidTenantEntity()
    {
        Id = Guid.CreateVersion7();
    }

    /// <summary>Same id the constructor would assign - use when you need it before the entity exists (e.g. an idempotency key).</summary>
    public static Guid NewId() => Guid.CreateVersion7();
}
