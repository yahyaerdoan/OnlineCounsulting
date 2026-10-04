using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.ListReferrals;
using OnlineConsulting.SharedKernel.Identity;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Referrals;

public class ListReferrals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/referrals/query", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("ListReferrals")
            .WithDescription("Returns all referrals (Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with the referrer's and referred user's email and user name for the referrals on the page.");
    }

    private static async Task<IResult> Handle(ISender sender, IUserContactReader contactReader, HttpContext httpContext, [AsParameters] ListQueryParameters query,
        [FromBody] DynamicQuery? dynamicQuery)
    {
        var referralsResult = await sender.Send(new ListReferralsQuery(query.ToPageRequest(), dynamicQuery));

        if (!referralsResult.IsSuccessful || referralsResult.Data is null)
        {
            return referralsResult.ToEnvelopedResult(httpContext);
        }

        var userIds = referralsResult.Data.Items.SelectMany(r => new[] { r.ReferrerUserId, r.ReferredUserId }).Distinct().ToList();
        var users = (await contactReader.GetContactsAsync(userIds, httpContext.RequestAborted)).ToDictionary(c => c.Id);

        var responseItems = referralsResult.Data.Items.Select(r =>
        {
            var referrer = users.GetValueOrDefault(r.ReferrerUserId);
            var referred = users.GetValueOrDefault(r.ReferredUserId);
            return new AdminReferralResponse(r.Id, r.Code, r.Status, r.RewardAmount, r.RewardedAt, r.ReferrerUserId, referrer?.Email, referrer?.UserName,
                r.ReferredUserId, referred?.Email, referred?.UserName);
        }).ToList();

        var response = new Paginate<AdminReferralResponse>
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
