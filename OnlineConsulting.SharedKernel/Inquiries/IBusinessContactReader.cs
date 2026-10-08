namespace OnlineConsulting.SharedKernel.Inquiries;

/// <summary>Cross-module read of a tenant's contact details; implemented by the Inquiries module.</summary>
public interface IBusinessContactReader
{
    /// <summary>The tenant's contact details, or null when it hasn't filled in its Contact page.</summary>
    Task<BusinessContact?> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
