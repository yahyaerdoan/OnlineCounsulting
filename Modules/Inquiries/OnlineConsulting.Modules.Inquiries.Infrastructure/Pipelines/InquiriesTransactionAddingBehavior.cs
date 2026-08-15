using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Pipelines;

// See Identity's IdentityTransactionAddingBehavior for why this thin per-module subclass exists.
// Not currently used by any Inquiries handler (Submit/Subscribe rely on enqueue-before-AddAsync
// ordering instead, see SubmitMessageCommand) - registered for the next handler that genuinely
// needs more than one SaveChanges call.
public class InquiriesTransactionAddingBehavior<TRequest, TResponse>(InquiriesDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, InquiriesDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
