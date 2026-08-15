using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Pipelines;

/// <summary>Thin per-module subclass closing EfTransactionAddingBehavior's TContext to this module's DbContext so it can register as an open generic.</summary>
public class CommerceTransactionAddingBehavior<TRequest, TResponse>(CommerceDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, CommerceDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
