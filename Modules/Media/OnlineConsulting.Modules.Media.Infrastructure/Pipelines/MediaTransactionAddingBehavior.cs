using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Media.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Media.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior to this module's DbContext for open-generic registration; unused today, kept for template consistency.</summary>
public class MediaTransactionAddingBehavior<TRequest, TResponse>(MediaDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, MediaDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
