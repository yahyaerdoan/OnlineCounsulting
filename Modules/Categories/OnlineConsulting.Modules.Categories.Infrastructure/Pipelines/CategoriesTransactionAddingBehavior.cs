using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Pipelines;

/// <summary>Thin per-module subclass closing EfTransactionAddingBehavior's TContext to this module's DbContext so it can register as an open generic.</summary>
public class CategoriesTransactionAddingBehavior<TRequest, TResponse>(CategoriesDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, CategoriesDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
