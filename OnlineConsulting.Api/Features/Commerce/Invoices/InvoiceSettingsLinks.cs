using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.UpdateInvoiceSettings;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

public sealed class InvoiceSettingsLinks : LinkProvider<InvoiceSettingsResponse>
{
    protected override void AddLinks(InvoiceSettingsResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetInvoiceSettings")
            .AddIf(links.User.CanSend<UpdateInvoiceSettingsCommand>(), LinkRelations.Edit, "UpdateInvoiceSettings", HttpMethods.Put);
}
