using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior's TContext to this module's DbContext so it can be registered as an open generic. No current handler implements ITransactionAddRequest (every write here is single-SaveChanges) - registered for consistency with every other module's template.</summary>
public class SiteContentTransactionAddingBehavior<TRequest, TResponse>(SiteContentDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, SiteContentDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
