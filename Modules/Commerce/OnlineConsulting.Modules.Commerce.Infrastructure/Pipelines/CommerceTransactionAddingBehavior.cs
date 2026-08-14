using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Pipelines;

// See Identity's IdentityTransactionAddingBehavior for why this thin per-module subclass exists -
// closes EfTransactionAddingBehavior's TContext to this module's own DbContext so it can still be
// registered as an open generic (TRequest/TResponse) against IPipelineBehavior<,>.
public class CommerceTransactionAddingBehavior<TRequest, TResponse>(CommerceDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, CommerceDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
