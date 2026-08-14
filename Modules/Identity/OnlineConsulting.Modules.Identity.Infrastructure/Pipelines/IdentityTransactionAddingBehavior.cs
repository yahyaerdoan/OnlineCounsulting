using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Pipelines;

// EfTransactionAddingBehavior<,,TContext> can't be registered as an open generic directly (DI's
// AddTransient(typeof(IPipelineBehavior<,>), typeof(Impl<,,>)) requires every type parameter left
// open) - this closes TContext to this module's own DbContext so TRequest/TResponse can stay open.
public class IdentityTransactionAddingBehavior<TRequest, TResponse>(AppIdentityDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, AppIdentityDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
