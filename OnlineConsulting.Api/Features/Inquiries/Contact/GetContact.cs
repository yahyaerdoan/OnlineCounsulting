using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.GetContact;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Contact;

public class GetContact : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/contact", Handle)
            .WithTags("Inquiries/Contact")
            .WithName("GetContact")
            .WithDescription("Returns the company's contact information. Public - no login required.")
            .ProducesEnveloped<CompanyContactResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetContactQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
