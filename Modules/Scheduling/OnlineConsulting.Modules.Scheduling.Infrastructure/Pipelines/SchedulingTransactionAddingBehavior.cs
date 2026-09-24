using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Pipelines;

/// <summary>Closes EfTransactionAddingBehavior to this module's DbContext for open-generic registration; unused today since every Scheduling write is single-SaveChanges.</summary>
public class SchedulingTransactionAddingBehavior<TRequest, TResponse>(SchedulingDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, SchedulingDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
