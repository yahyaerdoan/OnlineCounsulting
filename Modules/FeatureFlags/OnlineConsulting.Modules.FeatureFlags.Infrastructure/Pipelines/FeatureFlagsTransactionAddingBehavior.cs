using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.Modules.FeatureFlags.Infrastructure.Pipelines;

/// <summary>Thin per-module subclass closing EfTransactionAddingBehavior's TContext to this module's DbContext so it can register as an open generic.</summary>
public class FeatureFlagsTransactionAddingBehavior<TRequest, TResponse>(FeatureFlagsDbContext context) : EfTransactionAddingBehavior<TRequest, TResponse, FeatureFlagsDbContext>(context)
    where TRequest : IRequest<TResponse>, ITransactionAddRequest
    where TResponse : IOperationResult;
