namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module read access to a user's email, without referencing Identity's Domain/Application types (see IUserExistenceReader).</summary>
public interface IUserContactReader
{
    /// <summary>Null if no non-deleted User row exists for the given id.</summary>
    Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default);
}
