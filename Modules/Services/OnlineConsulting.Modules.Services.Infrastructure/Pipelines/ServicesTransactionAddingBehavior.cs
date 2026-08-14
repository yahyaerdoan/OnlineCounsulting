using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Services.Infrastructure.Pipelines;

// See Identity's IdentityTransactionAddingBehavior for why this thin per-module subclass exists -
// closes EfTransactionAddingBehavior's TContext to this module's own DbContext so it can still be
// registered as an open generic (TRequest/TResponse) against IPipelineBehavior<,>.
public class ServicesTransactionAddingBehavior<TRequest, TResponse>(ServicesDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, ServicesDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
