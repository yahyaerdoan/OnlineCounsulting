using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Contact;

public interface IContactService
{
    Task<ApiEnvelope> SubmitMessageAsync(CreateMessageViewModel model, CancellationToken cancellationToken = default);
}
