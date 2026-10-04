using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.CrossCuttingConcernLayer.Slugs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Services.Constants;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.CreateService;

public record CreateServiceCommand(Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment = false, bool IsEmergencyAvailable = false, Guid? CoverMediaAssetId = null, string PriceType = ServicePriceTypes.Fixed, decimal? PriceMax = null, string Kind = ServiceKinds.Booking)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Add, GlobalOperationClaims.SuperAdmin];
}

public class CreateServiceHandler(IServiceRepository repository) : IRequestHandler<CreateServiceCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var slug = await SlugGenerator.GenerateUniqueAsync(request.Title, async prefix => await repository.Query().Where(s => s.Slug.StartsWith(prefix)).Select(s => s.Slug).ToListAsync(cancellationToken));

        var service = Service.Create(request.CategoryId, request.Title, slug, request.Description, request.DetailedDescription, request.Kind,
            new ServicePrice(request.Price, request.PriceType, request.PriceMax, request.DiscountRate, request.TaxRate));
        service.SetOptions(request.FeaturedArea, request.RequiresPrepayment, request.IsEmergencyAvailable, request.CoverMediaAssetId);

        _ = await repository.AddAsync(service, cancellationToken: cancellationToken);

        return Result.Created(service.Id, "Service created successfully.");
    }
}
