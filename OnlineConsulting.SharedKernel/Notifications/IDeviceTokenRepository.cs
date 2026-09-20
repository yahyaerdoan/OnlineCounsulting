namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Narrow port for mobile push device tokens - implemented by Identity.Infrastructure, consumed by the push sender without a project reference to Identity.Application (same seam as IEmailOutboxWriter).</summary>
public interface IDeviceTokenRepository
{
    Task RegisterAsync(Guid userId, string token, string platform, CancellationToken cancellationToken = default);

    Task RemoveAsync(string token, CancellationToken cancellationToken = default);

    Task<List<string>> GetTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
