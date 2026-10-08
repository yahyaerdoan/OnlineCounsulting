using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Contracts;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices.GetInvoiceSettings;

public record GetInvoiceSettingsQuery : IRequest<OperationDataResult<InvoiceSettingsResponse>>, ISecureAddRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class GetInvoiceSettingsHandler(IInvoiceSettingsRepository repository, ITenantProvider tenantProvider)
    : IRequestHandler<GetInvoiceSettingsQuery, OperationDataResult<InvoiceSettingsResponse>>
{
    public async Task<OperationDataResult<InvoiceSettingsResponse>> Handle(GetInvoiceSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await repository.FindForTenantAsync(tenantProvider.TenantId, cancellationToken);
        return Result.Success(new InvoiceSettingsResponse(settings?.PaymentTermsDays ?? InvoiceSettings.DefaultPaymentTermsDays), "Invoice settings retrieved successfully.");
    }
}
