namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Narrow port for mobile push-notification device tokens - implemented by Identity.Infrastructure (which owns the actual table), consumed by anything that needs to reach a user by push (currently the Notifications project's push sender) without taking a project reference on Identity.Application, same seam as IEmailOutboxWriter.</summary>
public interface IDeviceTokenRepository
{
    Task RegisterAsync(Guid userId, string token, string platform, CancellationToken cancellationToken = default);

    Task RemoveAsync(string token, CancellationToken cancellationToken = default);

    Task<List<string>> GetTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
