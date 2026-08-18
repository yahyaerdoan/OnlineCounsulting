using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Pipelines;

public class ReferralsTransactionAddingBehavior<TRequest, TResponse>(ReferralsDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, ReferralsDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
