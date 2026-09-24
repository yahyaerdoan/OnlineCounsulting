using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior's TContext to this module's DbContext; no handler implements ITransactionAddRequest yet, registered for consistency with the template.</summary>
public class SiteContentTransactionAddingBehavior<TRequest, TResponse>(SiteContentDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, SiteContentDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
