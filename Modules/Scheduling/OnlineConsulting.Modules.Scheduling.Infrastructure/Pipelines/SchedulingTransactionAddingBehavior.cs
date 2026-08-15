using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior's TContext to this module's DbContext so it can be registered as an open generic. No current handler implements ITransactionAddRequest yet (every Scheduling write is single-SaveChanges) - registered for when a multi-write slice needs it.</summary>
public class SchedulingTransactionAddingBehavior<TRequest, TResponse>(SchedulingDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, SchedulingDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
