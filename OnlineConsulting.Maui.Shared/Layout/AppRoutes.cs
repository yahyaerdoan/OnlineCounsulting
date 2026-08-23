namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>UI route paths for C# call sites. @page directives keep their own literal (can't
/// reference a constant) - AdminHome=Dashboard.razor, Login=each host's Login.razor, NotFound=NotFound.razor.</summary>
public static class AppRoutes
{
    public const string AdminHome = "/admin";
    public const string Login = "/login";
    public const string Logout = "/logout";
    public const string NotFound = "/not-found";
}
