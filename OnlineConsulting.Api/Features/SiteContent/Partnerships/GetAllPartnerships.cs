using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.GetAllPartnerships;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class GetAllPartnerships : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/partnerships", Handle)
            .WithTags("SiteContent/Partnerships")
            .WithName("GetAllPartnerships")
            .WithDescription("Returns the tenant's partnership showcase entries, with their social links. Public - no login required. Returns an empty list when the tenant's Partnerships feature flag is off.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllPartnershipsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
