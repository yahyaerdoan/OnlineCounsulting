using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.GetAllReferralsPaged;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;
using PageRequest = Core.ApplicationLayer.Requests.Page.PageRequest;

namespace OnlineConsulting.Api.Features.Referrals;

/// <summary>Adds referrer/referred display names on top of the paged Application-layer response, same
/// enrichment shape GetAllOrdersAdminPaged.cs already uses for the identical need.</summary>
public record AdminReferralResponse(Guid Id, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt, Guid ReferrerUserId, string? ReferrerEmail, string? ReferrerName, Guid ReferredUserId, string? ReferredEmail, string? ReferredName);

public class GetAllReferralsPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/referrals/query", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("GetAllReferralsPaged")
            .WithDescription("Returns all referrals (Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with referrer/referred display names.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var referralsResult = await sender.Send(new GetAllReferralsPagedQuery(query.ToPageRequest(), dynamicQuery));
        if (!referralsResult.IsSuccessful || referralsResult.Data is null)
        {
            return referralsResult.ToEnvelopedResult(httpContext);
        }

        // Unbounded - this is a lookup for every referral's parties, not a paged listing of its own.
        var usersResult = await sender.Send(new GetAllUsersQuery(new PageRequest { PageIndex = 0, PageSize = int.MaxValue }));
        var usersById = (usersResult.IsSuccessful ? usersResult.Data?.Items : null)?.ToDictionary(u => u.Id) ?? [];

        var responseItems = referralsResult.Data.Items.Select(r =>
        {
            _ = usersById.TryGetValue(r.ReferrerUserId, out var referrer);
            _ = usersById.TryGetValue(r.ReferredUserId, out var referred);
            return new AdminReferralResponse(r.Id, r.Code, r.Status, r.RewardAmount, r.RewardedAt, r.ReferrerUserId, referrer?.Email, referrer?.UserName, r.ReferredUserId, referred?.Email, referred?.UserName);
        }).ToList();

        var response = new Core.PersistenceLayer.Pagings.Paging.Paginate<AdminReferralResponse>
        {
            Items = responseItems,
            Index = referralsResult.Data.Index,
            Size = referralsResult.Data.Size,
            Count = referralsResult.Data.Count,
            Pages = referralsResult.Data.Pages,
        };

        return Result.Success(response, "Referrals retrieved successfully.").ToEnvelopedResult(httpContext);
    }
}
