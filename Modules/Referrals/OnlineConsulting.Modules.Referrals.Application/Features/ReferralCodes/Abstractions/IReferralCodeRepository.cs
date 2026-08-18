using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;

public interface IReferralCodeRepository : IAsyncRepository<ReferralCode, Guid>
{
}
