using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.Repositories;

public class ReferralCodeRepository(ReferralsDbContext context) : EfRepositoryBase<ReferralCode, Guid, ReferralsDbContext>(context), IReferralCodeRepository
{
}
