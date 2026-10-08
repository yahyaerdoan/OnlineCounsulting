namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Name and email for addressing a user.</summary>
public sealed record UserContact(Guid Id, string? Email, string FirstName, string LastName, string? UserName)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
