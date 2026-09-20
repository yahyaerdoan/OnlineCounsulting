using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/users/{id:guid}", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetUserById")
            .WithDescription("Returns a single user by id.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetUserByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
