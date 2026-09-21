using ResultHandler.Core.Base;

namespace OnlineConsulting.UserInterface.Features.Account;

/// <summary>All authentication orchestration - credentials are verified only via the Api, then this app's own cookie is signed in locally from the result.</summary>
public interface IAccountService
{
    Task<OperationResult> RegisterAsync(string firstName, string lastName, string userName, string email, string password, CancellationToken cancellationToken = default);
    Task<LoginResult> LoginAsync(string userNameOrEmail, string password, bool rememberMe, CancellationToken cancellationToken = default);
    Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default);
}

/// <summary>Lets the controller pick a post-login redirect target (Admin dashboard vs. user dashboard) without
/// re-deriving "is this user an admin" itself - AccountService already knows the roles from /api/users/me.</summary>
public record LoginResult(OperationResult Result, bool IsAdmin);
