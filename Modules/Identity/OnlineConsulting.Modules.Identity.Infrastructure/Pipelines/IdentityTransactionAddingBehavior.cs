using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior's TContext to this module's DbContext so it can be registered as an open generic.</summary>
public class IdentityTransactionAddingBehavior<TRequest, TResponse>(AppIdentityDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, AppIdentityDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
