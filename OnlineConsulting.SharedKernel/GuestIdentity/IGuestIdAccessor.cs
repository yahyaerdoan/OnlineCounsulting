namespace OnlineConsulting.SharedKernel.GuestIdentity;

/// <summary>The anonymous shopper's id, kept in a cookie so a guest can fill a basket before signing in.</summary>
public interface IGuestIdAccessor
{
    /// <summary>Reads the guest id cookie if present, otherwise issues and writes a new one; for anonymous flows only.</summary>
    Guid GetOrCreateGuestId();

    /// <summary>Reads the guest id cookie without creating one; returns null if absent or invalid.</summary>
    Guid? TryGetGuestId();

    /// <summary>Called after login once the guest basket has been merged into the user's own basket.</summary>
    void ClearGuestId();
}
