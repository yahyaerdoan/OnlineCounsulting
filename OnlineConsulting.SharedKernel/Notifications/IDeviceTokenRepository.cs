namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Narrow port for mobile push device tokens - implemented by Identity.Infrastructure, consumed by the push sender without a project reference to Identity.Application (same seam as IEmailOutboxWriter).</summary>
public interface IDeviceTokenRepository
{
    /// <summary>Adds the token, or moves it to this user when the device was registered to someone else.</summary>
    Task RegisterAsync(Guid userId, string token, string platform, CancellationToken cancellationToken = default);

    /// <summary>Forgets the token; a no-op when it isn't registered.</summary>
    Task RemoveAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Every device token registered for the user; empty when none.</summary>
    Task<List<string>> GetTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
