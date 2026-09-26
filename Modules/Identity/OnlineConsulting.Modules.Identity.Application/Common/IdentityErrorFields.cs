using Microsoft.AspNetCore.Identity;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Maps ASP.NET Identity error codes to the request property the client sent, so forms can show each message next to its input.</summary>
public static class IdentityErrorFields
{
    /// <summary>A null field name sends that error group to the "" key (not tied to an input); so does any code with no known field.</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ToFieldErrors(this IdentityResult result, string? passwordField = null, string? userNameField = null, string? emailField = null, string? roleNameField = null)
        => result.Errors
            .GroupBy(error => FieldFor(error.Code, passwordField, userNameField, emailField, roleNameField) ?? string.Empty)
            .ToDictionary(group => group.Key, IReadOnlyList<string> (group) => [.. group.Select(error => error.Description).Distinct()]);

    private static string? FieldFor(string code, string? passwordField, string? userNameField, string? emailField, string? roleNameField)
        => code switch
        {
            _ when code.StartsWith("Password", StringComparison.Ordinal) => passwordField,
            nameof(IdentityErrorDescriber.DuplicateEmail) or nameof(IdentityErrorDescriber.InvalidEmail) => emailField,
            nameof(IdentityErrorDescriber.DuplicateUserName) or nameof(IdentityErrorDescriber.InvalidUserName) => userNameField,
            nameof(IdentityErrorDescriber.DuplicateRoleName) or nameof(IdentityErrorDescriber.InvalidRoleName) => roleNameField,
            _ => null,
        };
}
