using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.UpdateContact;

namespace OnlineConsulting.Api.Features.Inquiries.Contact;

public sealed class CompanyContactLinks : LinkProvider<CompanyContactResponse>
{
    protected override void AddLinks(CompanyContactResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetContact")
            .AddIf(links.User.CanSend<UpdateContactCommand>(), LinkRelations.Edit, "UpdateContact", HttpMethods.Put);
}
