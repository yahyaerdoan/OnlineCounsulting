namespace OnlineConsulting.SharedKernel.CurrentUser;

public interface ICurrentUserAccessor
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
}
