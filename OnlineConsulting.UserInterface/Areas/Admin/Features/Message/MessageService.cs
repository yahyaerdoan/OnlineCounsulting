using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Message;

public class MessageService(IApiClient apiClient) : IMessageService
{
    private const string MessagesPath = "/api/inquiries/messages";

    /// <summary>The Api paginates GetMessages - the legacy admin screen has no pagination UI, so this requests
    /// a large single page to preserve the old "show everything" behavior without redesigning the view.</summary>
    public async Task<List<MessageListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<PaginatedMessages>($"{MessagesPath}?index=0&size=1000", cancellationToken);
        var items = result.ResultData?.Items ?? [];
        return items.Select(m => new MessageListItemViewModel(m.Id, m.FirstName, m.LastName, m.Email, m.Subject, m.Description, m.CreatedDate)).ToList();
    }

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{MessagesPath}/{id}", cancellationToken);

    private record MessageResponse(Guid Id, string FirstName, string LastName, string Email, string Subject, string Description, DateTimeOffset CreatedDate);
    private record PaginatedMessages(List<MessageResponse> Items, int Index, int Size, int Count, int Pages);
}
