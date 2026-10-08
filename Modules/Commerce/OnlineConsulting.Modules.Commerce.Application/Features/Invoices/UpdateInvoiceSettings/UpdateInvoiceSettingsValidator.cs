using FluentValidation;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.UpdateInvoiceSettings;

public class UpdateInvoiceSettingsValidator : AbstractValidator<UpdateInvoiceSettingsCommand>
{
    public UpdateInvoiceSettingsValidator()
    {
        _ = RuleFor(x => x.PaymentTermsDays).InclusiveBetween(0, InvoiceSettings.MaxPaymentTermsDays);
    }
}
