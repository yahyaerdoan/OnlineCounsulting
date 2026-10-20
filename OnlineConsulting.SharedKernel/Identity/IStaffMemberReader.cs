namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module check that a user is on the current tenant's team, not a customer.</summary>
public interface IStaffMemberReader
{
    /// <summary>True for an active user of the current tenant holding a role other than User.</summary>
    Task<bool> IsStaffAsync(Guid userId, CancellationToken cancellationToken = default);
}
