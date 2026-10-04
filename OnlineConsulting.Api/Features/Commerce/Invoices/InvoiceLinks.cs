using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceForStaff;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.MarkInvoicePaid;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.VoidInvoice;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Api.Features.Commerce.Invoices;

/// <summary>
/// The owner gets the customer routes (view, PDF, pay while open with something to pay); staff viewing someone else's invoice get the
/// staff routes (view, PDF, mark paid and void while open), each only when their permissions allow it.
/// </summary>
public sealed class InvoiceLinks : LinkProvider<InvoiceResponse>
{
    protected override void AddLinks(InvoiceResponse resource, HateoasLinkBuilder links)
    {
        var id = new { id = resource.Id };
        var open = InvoiceRules.IsOpen(resource.Status);

        if (links.User.IsUser(resource.UserId))
        {
            _ = links
                .Self("GetMyInvoice", id)
                .AddCustom(Rels.Pdf, "GetMyInvoicePdf", HttpMethods.Get, id)
                .AddCustomIf(InvoiceRules.CanBePaidByCustomer(resource.Status, resource.Total), Rels.Pay, "PayMyInvoice", HttpMethods.Post, id);
            return;
        }

        if (!links.User.CanSend<GetInvoiceForStaffQuery>())
        {
            return;
        }

        _ = links
            .Self("GetInvoiceForStaff", id)
            .AddCustom(Rels.Pdf, "GetInvoicePdfForStaff", HttpMethods.Get, id)
            .AddCustomIf(open && links.User.CanSend<MarkInvoicePaidCommand>(), Rels.MarkPaid, "MarkInvoicePaid", HttpMethods.Post, id)
            .AddCustomIf(open && links.User.CanSend<VoidInvoiceCommand>(), Rels.Void, "VoidInvoice", HttpMethods.Post, id);
    }
}
