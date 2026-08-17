using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Contact;

public class ContactService(IApiClient apiClient) : IContactService
{
    public Task<ApiEnvelope> SubmitMessageAsync(CreateMessageViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync("/api/inquiries/messages", new
        {
            model.FirstName,
            model.LastName,
            model.Email,
            model.Subject,
            model.Description,
        }, cancellationToken);
}
