using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Domain;

/// <summary>A teammate invite; its token alone resolves the tenant on acceptance, so it is exempt from the EF tenant query filter.</summary>
public class Invite : SequentialGuidTenantEntity
{
    private Invite()
    {
    }

    public string Email { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;

    /// <summary>One of <see cref="InviteStatuses"/>.</summary>
    public string Status { get; private set; } = InviteStatuses.Pending;

    public DateTime ExpiresAt { get; private set; }
    public Guid InvitedByUserId { get; private set; }
    public DateTime? AcceptedAt { get; private set; }

    public bool IsPending => Status == InviteStatuses.Pending;

    /// <summary>Creates a pending invite for <paramref name="tenantId"/>.</summary>
    public static Invite Create(Guid tenantId, string email, string token, string roleName, DateTime expiresAt, Guid invitedByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        return new Invite
        {
            TenantId = tenantId,
            Email = email,
            Token = token,
            RoleName = roleName,
            ExpiresAt = expiresAt,
            InvitedByUserId = invitedByUserId,
        };
    }

    /// <summary>Past its expiry time at <paramref name="now"/>.</summary>
    public bool IsExpiredAt(DateTime now) => ExpiresAt < now;

    /// <summary>Requires <see cref="IsPending"/> and not expired at <paramref name="acceptedAt"/>.</summary>
    public void Accept(DateTime acceptedAt)
    {
        EnsurePending(nameof(Accept));
        if (IsExpiredAt(acceptedAt))
        {
            throw new InvalidOperationException($"Invite {Id} expired at {ExpiresAt:u}.");
        }

        Status = InviteStatuses.Accepted;
        AcceptedAt = acceptedAt;
    }

    /// <summary>Requires <see cref="IsPending"/>.</summary>
    public void Revoke()
    {
        EnsurePending(nameof(Revoke));
        Status = InviteStatuses.Revoked;
    }

    /// <summary>Marks a pending invite whose time ran out. Requires <see cref="IsPending"/>.</summary>
    public void Expire()
    {
        EnsurePending(nameof(Expire));
        Status = InviteStatuses.Expired;
    }

    private void EnsurePending(string action)
    {
        if (!IsPending)
        {
            throw new InvalidOperationException($"{action} needs a pending invite, but {Id} is {Status}.");
        }
    }
}
