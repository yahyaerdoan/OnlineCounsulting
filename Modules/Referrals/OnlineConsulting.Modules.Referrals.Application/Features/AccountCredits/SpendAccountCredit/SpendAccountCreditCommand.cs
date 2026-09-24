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

/// <summary>The one write path for debiting credit (balance is derived, never stored); callers must send this only after the thing it's paying for has already succeeded, since it can't be rolled back.</summary>
public record SpendAccountCreditCommand(Guid UserId, decimal Amount, string Reason, string SourceType, Guid SourceId) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class SpendAccountCreditHandler(IAccountCreditRepository creditRepository) : IRequestHandler<SpendAccountCreditCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(SpendAccountCreditCommand request, CancellationToken cancellationToken)
    {
        var entries = await creditRepository.GetListAsync(c => c.UserId == request.UserId, orderBy: q => q.OrderBy(c => c.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var balance = entries.Items.Sum(c => c.Amount);

        if (request.Amount > balance)
        {
            return Result.BadRequest<Guid>(ReferralsMessages.InsufficientCredit);
        }

        var entry = new AccountCredit
        {
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
