using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.GetModuleOfferingById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetModuleOfferingById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/tenancy/admin/module-offerings/{id:guid}", Handle)
            .WithTags("Tenancy")
            .RequireAuthorization()
            .WithName("GetModuleOfferingById")
            .WithDescription("Returns a single module offering, including hidden ones (SuperAdmin).");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetModuleOfferingByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
