using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;

public interface IAccountCreditRepository : IAsyncRepository<AccountCredit, Guid>
{
}
