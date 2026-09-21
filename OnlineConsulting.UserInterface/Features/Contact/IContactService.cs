using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Contact;

public interface IContactService
{
    /// <summary>Submits a public contact message.</summary>
    Task<ApiEnvelope> SubmitMessageAsync(CreateMessageViewModel model, CancellationToken cancellationToken = default);
}
