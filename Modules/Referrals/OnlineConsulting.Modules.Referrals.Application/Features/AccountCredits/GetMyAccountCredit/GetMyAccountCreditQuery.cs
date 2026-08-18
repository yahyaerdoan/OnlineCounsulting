using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;

public record GetMyAccountCreditQuery(Guid UserId) : IRequest<OperationDataResult<AccountCreditSummaryResponse>>;

public record AccountCreditSummaryResponse(decimal Balance, List<AccountCreditResponse> Entries);

public class GetMyAccountCreditHandler(IAccountCreditRepository repository) : IRequestHandler<GetMyAccountCreditQuery, OperationDataResult<AccountCreditSummaryResponse>>
{
    public async Task<OperationDataResult<AccountCreditSummaryResponse>> Handle(GetMyAccountCreditQuery request, CancellationToken cancellationToken)
    {
        var entries = await repository.GetListAsync(c => c.UserId == request.UserId, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var entryResponses = entries.Items.Select(AccountCreditResponse.FromDomain).ToList();
        var balance = entries.Items.Sum(c => c.Amount);

        return Result.Success(new AccountCreditSummaryResponse(balance, entryResponses), "Account credit retrieved successfully.");
    }
}
