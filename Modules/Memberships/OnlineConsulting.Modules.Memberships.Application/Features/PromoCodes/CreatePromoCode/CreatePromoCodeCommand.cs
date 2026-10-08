using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Constants;
using OnlineConsulting.Modules.Memberships.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.CreatePromoCode;

public record CreatePromoCodeCommand(string Code, string DiscountType, decimal DiscountValue, int? MaxRedemptions, DateTimeOffset? ExpiresAt, Guid? MembershipPlanId)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Add];
}

public class CreatePromoCodeHandler(IPromoCodeRepository repository) : IRequestHandler<CreatePromoCodeCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = PromoCode.Normalize(request.Code);

        var exists = await repository.AnyAsync(p => p.Code == normalizedCode, cancellationToken: cancellationToken);

        if (exists)
        {
            return Result.Conflict<Guid>(PromoCodeMessages.CodeAlreadyExists);
        }

        var promoCode = PromoCode.Create(request.Code, request.DiscountType, request.DiscountValue, request.MaxRedemptions, request.ExpiresAt, request.MembershipPlanId);

        _ = await repository.AddAsync(promoCode, cancellationToken: cancellationToken);

        return Result.Created(promoCode.Id, "Promo code created successfully.");
    }
}
