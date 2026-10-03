using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Memberships;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Billing;

/// <summary>Only an Active membership discounts services - Paused, PastDue and Cancelled ones don't.</summary>
public class MemberDiscountReader(MembershipsDbContext context) : IMemberDiscountReader
{
    public async Task<MemberDiscount?> GetActiveDiscountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var discount = await (
            from membership in context.CustomerMemberships
            join plan in context.MembershipPlans on membership.MembershipPlanId equals plan.Id
            where membership.UserId == userId && membership.Status == CustomerMembershipStatuses.Active && plan.DiscountPercent > 0
            orderby plan.DiscountPercent descending
            select new MemberDiscount(plan.Name, plan.DiscountPercent))
            .FirstOrDefaultAsync(cancellationToken);

        return discount;
    }
}
