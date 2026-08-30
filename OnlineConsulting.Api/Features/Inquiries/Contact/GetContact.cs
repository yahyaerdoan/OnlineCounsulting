using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.GetContact;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Inquiries.Contact;

public class GetContact : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/contact", Handle)
            .WithTags("Inquiries/Contact")
            .WithName("GetContact")
            .WithDescription("Returns the company's contact information. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetContactQuery());
        return result
            .OnSuccess(contact => contact.Links = BuildLinks(httpContext, linkGenerator))
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator)
        => httpContext.Links(linkGenerator)
            .Add("self", "GetContact", "GET")
            .Add("edit", "UpdateContact", "PUT")
            .Build();
}
