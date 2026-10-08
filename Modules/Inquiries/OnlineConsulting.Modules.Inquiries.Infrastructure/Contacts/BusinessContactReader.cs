using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Inquiries;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Contacts;

/// <summary>Reads by explicit tenant rather than the ambient one, so background jobs (invoice emails) get the right business.</summary>
public class BusinessContactReader(InquiriesDbContext context) : IBusinessContactReader
{
    public async Task<BusinessContact?> GetAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await context.CompanyContacts
            .IgnoreTenantFilter()
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Id)
            .Select(c => new BusinessContact(c.Email, c.Phone, c.Address))
            .FirstOrDefaultAsync(cancellationToken);
}
