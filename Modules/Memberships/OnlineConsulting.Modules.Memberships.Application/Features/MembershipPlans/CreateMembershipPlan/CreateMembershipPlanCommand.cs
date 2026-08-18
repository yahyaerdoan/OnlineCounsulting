using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.CreateMembershipPlan;

/// <summary>Creates the plan's provider-side product/price before persisting it - prices are immutable on the provider side, so this is the only place that ever mints one for a given plan (see MembershipPlan.ProviderPriceId).</summary>
public record CreateMembershipPlanCommand(string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Add];
}

public class CreateMembershipPlanHandler(IMembershipPlanRepository repository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<CreateMembershipPlanCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateMembershipPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new MembershipPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BillingCycle = request.BillingCycle,
            Price = request.Price,
            IncludedVisitsPerYear = request.IncludedVisitsPerYear,
            DiscountPercent = request.DiscountPercent,
            CreditAmount = request.CreditAmount,
            Benefits = request.Benefits,
        };

        var priceResult = await subscriptionGateway.EnsurePriceAsync(
            new EnsurePriceRequest(plan.Id.ToString(), plan.Name, plan.Price, "usd", plan.BillingCycle), cancellationToken);
        plan.ProviderProductId = priceResult.ProviderProductId;
        plan.ProviderPriceId = priceResult.ProviderPriceId;

        await repository.AddAsync(plan);

        return Result.Created(plan.Id, "Membership plan created successfully.");
    }
}
