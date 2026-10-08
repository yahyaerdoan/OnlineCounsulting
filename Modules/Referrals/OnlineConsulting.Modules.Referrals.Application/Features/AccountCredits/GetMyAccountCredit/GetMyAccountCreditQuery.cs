using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.GetMyAccountCredit;

public record GetMyAccountCreditQuery(Guid UserId) : IRequest<OperationDataResult<AccountCreditSummaryResponse>>;

public class GetMyAccountCreditHandler(IAccountCreditRepository repository) : IRequestHandler<GetMyAccountCreditQuery, OperationDataResult<AccountCreditSummaryResponse>>
{
    public async Task<OperationDataResult<AccountCreditSummaryResponse>> Handle(GetMyAccountCreditQuery request, CancellationToken cancellationToken)
    {
        var entries = await repository.GetAllAsync(c => c.UserId == request.UserId, cancellationToken: cancellationToken);
        var entryResponses = entries.Select(AccountCreditResponse.FromDomain).ToList();
        var balance = entries.Sum(c => c.Amount);

        return Result.Success(new AccountCreditSummaryResponse(balance, entryResponses), "Account credit retrieved successfully.");
    }
}
