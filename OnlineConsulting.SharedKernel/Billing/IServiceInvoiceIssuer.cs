namespace OnlineConsulting.SharedKernel.Billing;

public sealed record InvoiceLineInput(string Description, decimal Quantity, decimal UnitPrice, int TaxRate);

public sealed record ServiceInvoiceRequest(Guid AppointmentId, Guid CustomerUserId, string ServiceTitle, string? ServiceAddress, IReadOnlyList<InvoiceLineInput> Lines);

/// <summary>Cross-module port Scheduling calls when a visit is completed; implemented by Commerce, which owns invoices and payments.</summary>
public interface IServiceInvoiceIssuer
{
    /// <summary>Issues the visit's invoice and emails it to the customer; null when there were no billable lines. A zero total is issued as already paid.</summary>
    Task<Guid?> IssueForCompletedVisitAsync(ServiceInvoiceRequest request, CancellationToken cancellationToken = default);
}
