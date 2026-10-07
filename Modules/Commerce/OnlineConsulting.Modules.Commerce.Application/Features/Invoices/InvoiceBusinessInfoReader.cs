using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.Inquiries;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Invoices;

public class InvoiceBusinessInfoReader(ITenantBrandReader brandReader, IBusinessContactReader contactReader, IInvoiceSettingsRepository settingsRepository,
    IMediaAssetUrlReader mediaUrlReader)
    : IInvoiceBusinessInfoReader
{
    public async Task<InvoiceBusinessInfo> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var brand = await brandReader.GetAsync(tenantId, cancellationToken);
        var contact = await contactReader.GetAsync(tenantId, cancellationToken);
        var settings = await settingsRepository.FindForTenantAsync(tenantId, cancellationToken);
        var logoUrl = brand.LogoMediaAssetId is Guid logoId ? await mediaUrlReader.GetPublicUrlAsync(logoId, cancellationToken) : null;

        return new InvoiceBusinessInfo(brand.Name, logoUrl, contact?.Email, contact?.Phone, contact?.Address, settings?.PaymentTermsDays ?? InvoiceSettings.DefaultPaymentTermsDays);
    }
}
