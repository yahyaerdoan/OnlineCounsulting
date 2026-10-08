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
        _ = app.MapPost("/site-content/faq-items", Handle)
            .WithTags("SiteContent/FaqItems")
            .RequireAuthorization()
            .WithName("CreateFaqItem")
            .WithDescription("Creates a service-specific FAQ item.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateFaqItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateFaqItemRequest(Guid ServiceId, string Question, string Answer, int DisplayOrder = 0)
{
    public CreateFaqItemCommand ToCommand() => new(ServiceId, Question, Answer, DisplayOrder);
}
