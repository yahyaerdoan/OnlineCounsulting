using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Pipelines;

// See Identity's IdentityTransactionAddingBehavior for why this thin per-module subclass exists -
// closes EfTransactionAddingBehavior's TContext to this module's own DbContext so it can still be
// registered as an open generic (TRequest/TResponse) against IPipelineBehavior<,>.
public class CategoriesTransactionAddingBehavior<TRequest, TResponse>(CategoriesDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, CategoriesDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
