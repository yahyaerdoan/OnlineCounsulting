namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module read access to a user's email, without referencing Identity's Domain/Application types (see IUserExistenceReader).</summary>
public interface IUserContactReader
{
    /// <summary>Null if no non-deleted User row exists for the given id.</summary>
    Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Email and name for addressing a message; null if no non-deleted User row exists for the given id.</summary>
    Task<UserContact?> GetContactAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Contacts for a page of rows in one query; ids with no non-deleted User row are left out.</summary>
    Task<IReadOnlyList<UserContact>> GetContactsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>Ids of users whose name or email contains the term, for searching lists that only store a UserId; at most maxResults.</summary>
    Task<IReadOnlyList<Guid>> FindUserIdsAsync(string term, int maxResults = 200, CancellationToken cancellationToken = default);
}
