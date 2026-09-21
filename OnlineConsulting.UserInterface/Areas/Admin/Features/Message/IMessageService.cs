using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Message;

/// <summary>All Api orchestration for the admin Message screen (submitted contact-form messages).</summary>
public interface IMessageService
{
    /// <summary>Fetches all submitted contact-form messages.</summary>
    Task<List<MessageListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Deletes a message by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
