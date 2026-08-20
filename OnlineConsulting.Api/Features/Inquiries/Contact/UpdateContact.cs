using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.UpdateContact;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Contact;

public class UpdateContact : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/contact", Handle)
            .WithTags("Inquiries/Contact")
            .RequireAuthorization()
            .WithName("UpdateContact")
            .WithDescription("Creates or updates the company's contact information. Admin only.");
    }

    private static async Task<IResult> Handle([FromBody] UpdateContactCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
