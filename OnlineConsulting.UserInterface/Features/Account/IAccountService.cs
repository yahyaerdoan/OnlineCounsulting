using ResultHandler.Core.Base;

namespace OnlineConsulting.UserInterface.Features.Account;

/// <summary>All authentication orchestration for the account controller - credential verification happens only
/// via the Api (POST /api/auth/login), this app's own cookie is then signed in locally from the resulting user,
/// translating the Api's answer into a session rather than re-checking the password in-process. Controllers
/// depend on this alone, never on IApiClient directly.</summary>
public interface IAccountService
{
    Task<OperationResult> RegisterAsync(string firstName, string lastName, string userName, string email, string password, CancellationToken cancellationToken = default);
    Task<LoginResult> LoginAsync(string userNameOrEmail, string password, bool rememberMe, CancellationToken cancellationToken = default);
    Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default);
}

/// <summary>Lets the controller pick a post-login redirect target (Admin dashboard vs. user dashboard) without
/// re-deriving "is this user an admin" itself - AccountService already knows the roles from /api/users/me.</summary>
public record LoginResult(OperationResult Result, bool IsAdmin);
