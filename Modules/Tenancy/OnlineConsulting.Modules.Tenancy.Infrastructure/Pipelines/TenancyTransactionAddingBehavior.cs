using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Pipelines;

/// <summary>Thin per-module subclass closing EfTransactionAddingBehavior's TContext to this module's DbContext so it can register as an open generic.</summary>
public class TenancyTransactionAddingBehavior<TRequest, TResponse>(TenancyDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, TenancyDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
