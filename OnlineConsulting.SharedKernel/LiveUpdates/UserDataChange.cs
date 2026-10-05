namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>One user's data under one topic changed; clients refetch that topic from the Api - the signal carries no data.</summary>
public readonly record struct UserDataChange(Guid UserId, string Topic);
