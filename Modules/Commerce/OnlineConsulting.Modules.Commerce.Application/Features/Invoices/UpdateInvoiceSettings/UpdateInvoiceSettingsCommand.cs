using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.UpdateInvoiceSettings;

/// <summary>Sets the caller's business's invoicing preferences, creating its settings row on the first change.</summary>
public record UpdateInvoiceSettingsCommand(int PaymentTermsDays) : IRequest<OperationResult>, ISecureAddRequest, ICommerceTransactionRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class UpdateInvoiceSettingsHandler(IInvoiceSettingsRepository repository) : IRequestHandler<UpdateInvoiceSettingsCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateInvoiceSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await repository.GetAsync(s => true, cancellationToken: cancellationToken);

        if (settings is null)
        {
            _ = await repository.AddAsync(InvoiceSettings.Create(request.PaymentTermsDays), cancellationToken: cancellationToken);
        }
        else
        {
            settings.ChangePaymentTerms(request.PaymentTermsDays);
            _ = await repository.UpdateAsync(settings, cancellationToken: cancellationToken);
        }

        return Result.Success("Invoice settings updated.");
    }
}
