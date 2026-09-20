using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.SetPromoCodeActive;

public record SetPromoCodeActiveCommand(Guid Id, bool IsActive) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MembershipsOperationClaims.Admin, MembershipsOperationClaims.Write, MembershipsOperationClaims.Update];
}

public class SetPromoCodeActiveHandler(IPromoCodeRepository repository) : IRequestHandler<SetPromoCodeActiveCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetPromoCodeActiveCommand request, CancellationToken cancellationToken)
    {
        var promoCode = await repository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

        if (promoCode is null)
        {
            return Result.NotFound(string.Format(PromoCodeMessages.PromoCodeNotFoundFormat, request.Id));
        }

        promoCode.IsActive = request.IsActive;

        _ = await repository.UpdateAsync(promoCode);

        return Result.Success(request.IsActive ? "Promo code activated successfully." : "Promo code deactivated successfully.");
    }
}
