namespace OnlineConsulting.Modules.Identity.Application.Common.Templates;

/// <summary>One business an address has an account with, and where to sign in to it.</summary>
public record BusinessSignInLink(string BusinessName, string SignInUrl);
