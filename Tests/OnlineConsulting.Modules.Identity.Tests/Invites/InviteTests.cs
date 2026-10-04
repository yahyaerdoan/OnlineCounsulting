using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Tests.Invites;

public class InviteTests
{
    private static readonly DateTime Now = new(2026, 10, 3, 12, 0, 0, DateTimeKind.Utc);

    private static Invite Pending() => Invite.Create(Guid.NewGuid(), "tech@acme.test", "token-1", "Member", Now.AddDays(7), Guid.NewGuid());

    [Fact]
    public void Create_IsPendingForTheTenant()
    {
        var tenantId = Guid.NewGuid();

        var invite = Invite.Create(tenantId, "tech@acme.test", "token-1", "Member", Now.AddDays(7), Guid.NewGuid());

        Assert.True(invite.IsPending);
        Assert.Equal(tenantId, invite.TenantId);
        Assert.Null(invite.AcceptedAt);
    }

    [Theory]
    [InlineData("", "token", "Member")]
    [InlineData("a@b.test", " ", "Member")]
    [InlineData("a@b.test", "token", "")]
    public void Create_WithBlankValue_Throws(string email, string token, string role) =>
        Assert.ThrowsAny<ArgumentException>(() => Invite.Create(Guid.NewGuid(), email, token, role, Now, Guid.NewGuid()));

    [Fact]
    public void Accept_BeforeExpiry_Accepts()
    {
        var invite = Pending();

        invite.Accept(Now);

        Assert.Equal(InviteStatuses.Accepted, invite.Status);
        Assert.Equal(Now, invite.AcceptedAt);
    }

    [Fact]
    public void Accept_AfterExpiry_Throws() =>
        Assert.Throws<InvalidOperationException>(() => Pending().Accept(Now.AddDays(8)));

    [Fact]
    public void IsExpiredAt_ComparesWithExpiry()
    {
        var invite = Pending();

        Assert.False(invite.IsExpiredAt(Now));
        Assert.True(invite.IsExpiredAt(Now.AddDays(8)));
    }

    [Fact]
    public void Revoke_WhenPending_Revokes()
    {
        var invite = Pending();

        invite.Revoke();

        Assert.Equal(InviteStatuses.Revoked, invite.Status);
    }

    [Fact]
    public void Expire_WhenPending_Expires()
    {
        var invite = Pending();

        invite.Expire();

        Assert.Equal(InviteStatuses.Expired, invite.Status);
    }

    public static TheoryData<Action<Invite>> Actions => new()
    {
        i => i.Accept(Now),
        i => i.Revoke(),
        i => i.Expire(),
    };

    [Theory]
    [MemberData(nameof(Actions))]
    public void Action_WhenNotPending_Throws(Action<Invite> action)
    {
        var invite = Pending();
        invite.Revoke();

        _ = Assert.Throws<InvalidOperationException>(() => action(invite));
    }
}
