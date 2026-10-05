namespace OnlineConsulting.SharedKernel.Billing;

/// <summary>What Scheduling sends Commerce to invoice a completed visit.</summary>
public sealed record ServiceInvoiceRequest(Guid AppointmentId, Guid CustomerUserId, string ServiceTitle, string? ServiceAddress, IReadOnlyList<InvoiceLineInput> Lines);
