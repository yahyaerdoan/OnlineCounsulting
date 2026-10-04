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
        _ = app.MapPut("/contact", Handle)
            .WithTags("Inquiries/Contact")
            .RequireAuthorization()
            .WithName("UpdateContact")
            .WithDescription("Creates or updates the company's contact information. Admin only.");
    }

    private static async Task<IResult> Handle([FromBody] UpdateContactRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateContactRequest(string Email, string Phone, string Address, string Description, string WorkingHours)
{
    public UpdateContactCommand ToCommand() => new(Email, Phone, Address, Description, WorkingHours);
}
