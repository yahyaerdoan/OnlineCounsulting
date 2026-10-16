using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.SaveLegalPolicy;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LegalPolicies;

public class SaveLegalPolicy : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/legal-policy", Handle)
            .WithTags("SiteContent/LegalPolicy")
            .RequireAuthorization()
            .WithName("SaveLegalPolicy")
            .WithDescription("Creates or updates the site's privacy policy and terms of service.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] SaveLegalPolicyRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SaveLegalPolicyRequest(string? PrivacyPolicy, string? TermsOfService)
{
    public SaveLegalPolicyCommand ToCommand() => new(PrivacyPolicy, TermsOfService);
}
