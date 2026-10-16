using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.GetLegalPolicy;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LegalPolicies;

public class GetLegalPolicy : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/site-content/legal-policy", Handle)
            .WithTags("SiteContent/LegalPolicy")
            .WithName("GetLegalPolicy")
            .WithDescription("Returns the site's privacy policy and terms of service; ?document=privacy-policy or terms-of-service 404s while that one is empty. Public - no login required.")
            .ProducesEnveloped<LegalPolicyResponse>();
    }

    private static async Task<IResult> Handle(string? document, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetLegalPolicyQuery(document));
        return result.ToEnvelopedResult(httpContext);
    }
}
