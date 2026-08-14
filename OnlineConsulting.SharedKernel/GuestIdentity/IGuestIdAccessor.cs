namespace OnlineConsulting.SharedKernel.GuestIdentity;

public interface IGuestIdAccessor
{
    // Reads the guest id cookie if present; otherwise issues a new one and writes it to the
    // response. Callers only need this for anonymous flows - an authenticated caller should use
    // ICurrentUserAccessor instead.
    Guid GetOrCreateGuestId();

    Guid? TryGetGuestId();

    // Called after a successful login once the guest basket (if any) has been merged into the
    // user's own basket - the guest id no longer refers to anything worth keeping.
    void ClearGuestId();
}
