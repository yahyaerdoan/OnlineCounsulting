using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.SpendAccountCredit;

/// <summary>The one write path for debiting a customer's account credit - balance is derived (sum of ledger entries), never stored, so every spend must go through here to avoid a negative balance. Callers (e.g. Membership subscribe orchestration) must only send this after the thing the credit is paying for has already succeeded, since this command itself can't be rolled back once committed.</summary>
public record SpendAccountCreditCommand(Guid UserId, decimal Amount, string Reason, string SourceType, Guid SourceId) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class SpendAccountCreditHandler(IAccountCreditRepository creditRepository) : IRequestHandler<SpendAccountCreditCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(SpendAccountCreditCommand request, CancellationToken cancellationToken)
    {
        var entries = await creditRepository.GetListAsync(c => c.UserId == request.UserId, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var balance = entries.Items.Sum(c => c.Amount);

        if (request.Amount > balance)
        {
            return Result.BadRequest<Guid>(ReferralsMessages.InsufficientCredit);
        }

        var entry = new AccountCredit
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = -request.Amount,
            Reason = request.Reason,
            SourceType = request.SourceType,
            SourceId = request.SourceId,
        };

        _ = await creditRepository.AddAsync(entry);

        return Result.Created(entry.Id, "Account credit spent successfully.");
    }
}
