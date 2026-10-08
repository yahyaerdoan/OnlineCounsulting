namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>One line of a provider invoice, in the invoice currency.</summary>
public sealed record SubscriptionInvoiceLine(string Description, decimal Amount);
