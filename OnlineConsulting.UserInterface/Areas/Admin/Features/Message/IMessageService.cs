using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Message;

/// <summary>All Api orchestration for the admin Message screen (submitted contact-form messages).</summary>
public interface IMessageService
{
    Task<List<MessageListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
