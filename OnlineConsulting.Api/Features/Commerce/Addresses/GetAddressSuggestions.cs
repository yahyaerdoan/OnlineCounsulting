using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetAddressSuggestions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Commerce.Addresses;

public class GetAddressSuggestions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/addresses/suggestions", Handle)
            .WithTags("Commerce/Addresses")
            .RequireAuthorization()
            .WithName("GetAddressSuggestions")
            .WithDescription("US street address suggestions for type-ahead (signed-in users only, to protect the provider quota).");
    }

    private static async Task<IResult> Handle(string? text, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAddressSuggestionsQuery(text));
        return result.ToEnvelopedResult(httpContext);
    }
}
