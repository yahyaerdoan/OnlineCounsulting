using Core.PersistenceLayer.MultiTenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>A business's invoicing preferences; one row per tenant, created on its first change. Without a row the defaults apply.</summary>
public class InvoiceSettings : SequentialGuidTenantEntity
{
    public const int DefaultPaymentTermsDays = 14;
    public const int MaxPaymentTermsDays = 365;

    private InvoiceSettings()
    {
    }

    /// <summary>Days after issue a service-visit invoice falls due; 0 means due on receipt.</summary>
    public int PaymentTermsDays { get; private set; } = DefaultPaymentTermsDays;

    /// <summary>Requires 0 to <see cref="MaxPaymentTermsDays"/> days.</summary>
    public static InvoiceSettings Create(int paymentTermsDays)
    {
        var settings = new InvoiceSettings();
        settings.ChangePaymentTerms(paymentTermsDays);
        return settings;
    }

    /// <summary>Requires 0 to <see cref="MaxPaymentTermsDays"/> days.</summary>
    public void ChangePaymentTerms(int paymentTermsDays)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(paymentTermsDays);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(paymentTermsDays, MaxPaymentTermsDays);
        PaymentTermsDays = paymentTermsDays;
    }
}
