using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.UpdateServiceProcessStep;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public class UpdateServiceProcessStep : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/service-process-steps/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceProcessSteps")
            .RequireAuthorization()
            .WithName("UpdateServiceProcessStep")
            .WithDescription("Updates a service process step.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceProcessStepRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateServiceProcessStepRequest(string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateServiceProcessStepCommand ToCommand(Guid id) => new(id, Title, Description, Icon, IconColor, DisplayOrder, Metadata);
}
