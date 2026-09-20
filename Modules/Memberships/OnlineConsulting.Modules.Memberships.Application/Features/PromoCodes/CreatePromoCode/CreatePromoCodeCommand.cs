using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Constants;
using OnlineConsulting.Modules.Memberships.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.CreatePromoCode;

public record CreatePromoCodeCommand(string Code, string DiscountType, decimal DiscountValue, int? MaxRedemptions, DateTimeOffset? ExpiresAt, Guid? MembershipPlanId)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Add];
}

public class CreatePromoCodeHandler(IPromoCodeRepository repository) : IRequestHandler<CreatePromoCodeCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var exists = await repository.AnyAsync(p => p.Code == normalizedCode, cancellationToken: cancellationToken);

        if (exists)
        {
            return Result.BadRequest<Guid>(PromoCodeMessages.CodeAlreadyExists);
        }

        var promoCode = new PromoCode
        {
            Code = normalizedCode,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MaxRedemptions = request.MaxRedemptions,
            ExpiresAt = request.ExpiresAt,
            MembershipPlanId = request.MembershipPlanId,
        };

        _ = await repository.AddAsync(promoCode);

        return Result.Created(promoCode.Id, "Promo code created successfully.");
    }
}
