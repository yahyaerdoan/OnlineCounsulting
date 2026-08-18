using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Referrals.Domain;

namespace OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;

public interface IReferralRepository : IAsyncRepository<Referral, Guid>
{
}
