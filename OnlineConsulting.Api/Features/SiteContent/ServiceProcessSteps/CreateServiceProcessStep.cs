using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.CreateServiceProcessStep;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class CreateServiceProcessStep : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/service-process-steps", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .RequireAuthorization()
            .WithName("CreateServiceProcessStep")
            .WithDescription("Creates a step in the \"how you get our service\" homepage section.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceProcessStepRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateServiceProcessStepRequest(string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateServiceProcessStepCommand ToCommand() => new(Title, Description, Icon, IconColor, DisplayOrder, Metadata);
}
