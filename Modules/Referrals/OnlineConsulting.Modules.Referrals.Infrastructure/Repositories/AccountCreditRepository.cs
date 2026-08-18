using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Repositories;

public class AccountCreditRepository(ReferralsDbContext context) : EfRepositoryBase<AccountCredit, Guid, ReferralsDbContext>(context), IAccountCreditRepository
{
}
