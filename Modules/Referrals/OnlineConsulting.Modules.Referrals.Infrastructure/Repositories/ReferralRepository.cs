using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Repositories;

public class ReferralRepository(ReferralsDbContext context) : EfRepositoryBase<Referral, Guid, ReferralsDbContext>(context), IReferralRepository
{
}
