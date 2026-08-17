using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;

public class ContactService(IApiClient apiClient) : IContactService
{
    private const string ContactPath = "/api/contact";

    public async Task<ContactViewModel?> GetAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<CompanyContactResponse>(ContactPath, cancellationToken);
        var contact = result.ResultData;
        if (contact is null)
            return null;

        return new ContactViewModel
        {
            Email = contact.Email,
            Phone = contact.Phone,
            Address = contact.Address,
            Description = contact.Description,
            WorkingHours = contact.WorkingHours,
        };
    }

    public Task<ApiEnvelope> UpdateAsync(ContactViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync(ContactPath, new
        {
            model.Email,
            model.Phone,
            model.Address,
            model.Description,
            model.WorkingHours,
        }, cancellationToken);

    private record CompanyContactResponse(Guid Id, string Email, string Phone, string Address, string Description, string WorkingHours);
}
