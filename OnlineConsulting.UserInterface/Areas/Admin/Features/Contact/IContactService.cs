using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;

/// <summary>All Api orchestration for the admin Contact-information screen. There is no CreateAsync/DeleteAsync -
/// the Api's UpdateContact is an upsert for the single per-tenant CompanyContact row.</summary>
public interface IContactService
{
    Task<ContactViewModel?> GetAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(ContactViewModel model, CancellationToken cancellationToken = default);
}
