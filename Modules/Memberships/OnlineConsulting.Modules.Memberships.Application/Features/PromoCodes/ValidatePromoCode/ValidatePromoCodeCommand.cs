using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Constants;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Common;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.ValidatePromoCode;

public record ValidatePromoCodeCommand(Guid UserId, string Code, Guid MembershipPlanId) : IRequest<OperationDataResult<ValidatePromoCodeResult>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public record ValidatePromoCodeResult(bool IsValid, string? Error, decimal DiscountAmount, decimal FinalPrice);

/// <summary>Preview only - unlike SubscribeToMembershipCommand's use of the same evaluator, this never
/// touches PromoCode.RedemptionCount. IsValid=false is returned as a normal 200 (Result.Success), not
/// a BadRequest, so the UI can render the reason inline instead of a validation-error toast.</summary>
public class ValidatePromoCodeHandler(IMembershipPlanRepository planRepository, IPromoCodeRepository promoCodeRepository, ICustomerMembershipRepository membershipRepository)
    : IRequestHandler<ValidatePromoCodeCommand, OperationDataResult<ValidatePromoCodeResult>>
{
    public async Task<OperationDataResult<ValidatePromoCodeResult>> Handle(ValidatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetAsync(p => p.Id == request.MembershipPlanId, cancellationToken: cancellationToken);

        if (plan is null)
        {
            return Result.NotFound<ValidatePromoCodeResult>(string.Format(MembershipPlanMessages.MembershipPlanNotFoundFormat, request.MembershipPlanId));
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var promo = await promoCodeRepository.GetAsync(p => p.Code == normalizedCode, cancellationToken: cancellationToken);

        var alreadyRedeemed = promo is not null && await membershipRepository.AnyAsync(m => m.UserId == request.UserId && m.PromoCodeId == promo.Id, cancellationToken: cancellationToken);

        var (isValid, error, discountAmount) = PromoCodeEvaluator.Evaluate(promo, plan, alreadyRedeemed);

        var result = new ValidatePromoCodeResult(isValid, error, discountAmount, plan.Price - discountAmount);

        return Result.Success(result, isValid ? "Promo code applied successfully." : "Promo code is not valid.");
    }
}
