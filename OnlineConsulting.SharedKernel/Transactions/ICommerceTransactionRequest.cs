using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;

namespace OnlineConsulting.SharedKernel.Transactions;

/// <summary>Commands that write to the Commerce module's database; they run in a transaction on its DbContext.</summary>
public interface ICommerceTransactionRequest : ITransactionAddRequest;
