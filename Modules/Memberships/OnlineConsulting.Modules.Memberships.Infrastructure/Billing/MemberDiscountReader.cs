using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Memberships;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Billing;

/// <summary>Only an Active membership discounts services; Paused, PastDue and Cancelled ones don't.</summary>
public class MemberDiscountReader(MembershipsDbContext context) : IMemberDiscountReader
{
    public Task<MemberDiscount?> GetActiveDiscountAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.CustomerMemberships
            .Where(m => m.UserId == userId && m.Status == CustomerMembershipStatuses.Active)
            .Join(context.MembershipPlans.Where(p => p.DiscountPercent > 0), m => m.MembershipPlanId, p => p.Id, (membership, plan) => plan)
            .OrderByDescending(p => p.DiscountPercent)
            .Select(p => new MemberDiscount(p.Name, p.DiscountPercent))
            .FirstOrDefaultAsync(cancellationToken);
}
