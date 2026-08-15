using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Pipelines;

/// <summary>Not currently used by any handler; registered ahead of time for the next one that needs more than one SaveChanges call.</summary>
public class InquiriesTransactionAddingBehavior<TRequest, TResponse>(InquiriesDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, InquiriesDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
