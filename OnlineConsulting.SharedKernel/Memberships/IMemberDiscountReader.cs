namespace OnlineConsulting.SharedKernel.Memberships;

public sealed record MemberDiscount(string PlanName, decimal Percent);

/// <summary>Cross-module read of the discount a customer's active membership gives on services; implemented by the Memberships module.</summary>
public interface IMemberDiscountReader
{
    /// <summary>Null when the customer has no active membership, or its plan has no discount.</summary>
    Task<MemberDiscount?> GetActiveDiscountAsync(Guid userId, CancellationToken cancellationToken = default);
}
