namespace OnlineConsulting.SharedKernel.GuestIdentity;

public interface IGuestIdAccessor
{
    /// <summary>Reads the guest id cookie if present, otherwise issues and writes a new one; for anonymous flows only.</summary>
    Guid GetOrCreateGuestId();

    Guid? TryGetGuestId();

    /// <summary>Called after login once the guest basket has been merged into the user's own basket.</summary>
    void ClearGuestId();
}
