using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;

namespace OnlineConsulting.SharedKernel.Transactions;

/// <summary>Commands that write to the Tenancy module's database; they run in a transaction on its DbContext.</summary>
public interface ITenancyTransactionRequest : ITransactionAddRequest;
