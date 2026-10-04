using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.CreatePartnership;
using OnlineConsulting.Modules.SiteContent.Domain.Partnerships;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class CreatePartnership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/partnerships", Handle)
            .WithTags("SiteContent/Partnerships")
            .RequireAuthorization()
            .WithName("CreatePartnership")
            .WithDescription("Creates a partnership showcase entry.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePartnershipRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreatePartnershipRequest(string FirstName, string LastName, string? Email, string Title, string? CompanyName, string Description, string? WebsiteUrl, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null, string Kind = PartnershipKinds.Partner)
{
    public CreatePartnershipCommand ToCommand() => new(FirstName, LastName, Email, Title, CompanyName, Description, WebsiteUrl, PhotoMediaAssetId, DisplayOrder, Metadata, Kind);
}
