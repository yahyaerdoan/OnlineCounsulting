using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.CreateFaqItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FaqItems;

public class CreateFaqItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/faq-items", Handle)
            .WithTags("SiteContent/FaqItems")
            .RequireAuthorization()
            .WithName("CreateFaqItem")
            .WithDescription("Creates a service-specific FAQ item.");
    }

    private static async Task<IResult> Handle([FromBody] CreateFaqItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
