using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.DeleteService;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class DeleteService : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/services/{id:guid}", Handle)
            .WithTags("Services")
            .RequireAuthorization()
            .WithName("DeleteService")
            .WithDescription("Soft-deletes a service.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteServiceCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
