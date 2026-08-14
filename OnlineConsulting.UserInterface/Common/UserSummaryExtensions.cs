using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

namespace OnlineConsulting.UserInterface.Common;

/// <summary>
/// Views still bind to the legacy ResultUserDto shape; this adapts the Identity module's UserResponse
/// to it rather than changing every view's @model.
/// </summary>
public static class UserSummaryExtensions
{
    public static ResultUserDto ToResultUserDto(this UserResponse summary) => new()
    {
        Id = summary.Id,
        TenantId = summary.TenantId,
        Username = summary.UserName,
        FirstName = summary.FirstName,
        LastName = summary.LastName,
        Email = summary.Email,
        ImageUrl = summary.ImageUrl ?? string.Empty,
    };
}
