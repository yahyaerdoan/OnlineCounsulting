using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.CrossCuttingConcernLayer.Slugs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Constants;
using OnlineConsulting.Modules.Services.Application.Features.Services.Rules;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.UpdateService;

public record UpdateServiceCommand(Guid Id, Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment, bool IsEmergencyAvailable = false, Guid? CoverMediaAssetId = null, string PriceType = ServicePriceTypes.Fixed, decimal? PriceMax = null, string Kind = ServiceKinds.Booking)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Update, GlobalOperationClaims.SuperAdmin];
}

public class UpdateServiceHandler(IServiceRepository repository) : IRequestHandler<UpdateServiceCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);
        if (service is null)
        {
            return ServiceBusinessRules.ServiceNotFound(request.Id);
        }

        if (!string.Equals(service.Title, request.Title, StringComparison.Ordinal))
        {
            service.ChangeSlug(await SlugGenerator.GenerateUniqueAsync(request.Title,
                async prefix => await repository.Query().Where(s => s.Slug.StartsWith(prefix) && s.Id != request.Id).Select(s => s.Slug).ToListAsync(cancellationToken)));
        }

        service.UpdateDetails(request.CategoryId, request.Title, request.Description, request.DetailedDescription, request.Kind);
        service.ChangePricing(new ServicePrice(request.Price, request.PriceType, request.PriceMax, request.DiscountRate, request.TaxRate));
        service.SetOptions(request.FeaturedArea, request.RequiresPrepayment, request.IsEmergencyAvailable, request.CoverMediaAssetId);

        _ = await repository.UpdateAsync(service, cancellationToken: cancellationToken);

        return Result.Success("Service updated successfully.");
    }
}
