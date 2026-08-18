using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Services.Application.Features.CreateService;

public record CreateServiceCommand(Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment = false, bool IsEmergencyAvailable = false, Guid? CoverMediaAssetId = null, string PriceType = ServicePriceTypes.Fixed, decimal? PriceMax = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Add];
}

public class CreateServiceHandler(IServiceRepository repository) : IRequestHandler<CreateServiceCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var slug = await SlugGenerator.GenerateUniqueAsync(request.Title, candidate => repository.AnyAsync(s => s.Slug == candidate, cancellationToken: cancellationToken));

        var service = new Service
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            DetailedDescription = request.DetailedDescription,
            Price = request.Price,
            FeaturedArea = request.FeaturedArea,
            DiscountRate = request.DiscountRate,
            TaxRate = request.TaxRate,
            DiscountedPrice = ServicePriceCalculator.CalculateDiscountedPrice(request.Price, request.DiscountRate),
            RequiresPrepayment = request.RequiresPrepayment,
            IsEmergencyAvailable = request.IsEmergencyAvailable,
            CoverMediaAssetId = request.CoverMediaAssetId,
            PriceType = request.PriceType,
            PriceMax = request.PriceMax,
        };

        await repository.AddAsync(service);

        return Result.Created(service.Id, "Service created successfully.");
    }
}
