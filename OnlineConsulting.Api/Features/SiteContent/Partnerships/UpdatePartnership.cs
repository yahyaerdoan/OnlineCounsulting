using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.UpdatePartnership;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class UpdatePartnership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/partnerships/{id:guid}", Handle)
            .WithTags("SiteContent/Partnerships")
            .RequireAuthorization()
            .WithName("UpdatePartnership")
            .WithDescription("Updates a partnership showcase entry.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePartnershipRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdatePartnershipRequest(string FirstName, string LastName, string? Email, string Title, string? CompanyName, string Description, string? WebsiteUrl, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null, string Kind = PartnershipKinds.Partner)
{
    public UpdatePartnershipCommand ToCommand(Guid id) => new(id, FirstName, LastName, Email, Title, CompanyName, Description, WebsiteUrl, PhotoMediaAssetId, DisplayOrder, Metadata, Kind);
}
