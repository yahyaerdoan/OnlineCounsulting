using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Application.Features.Rules;
using OnlineConsulting.SharedKernel.Slugs;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Services.Application.Features.UpdateService;

public record UpdateServiceCommand(Guid Id, Guid CategoryId, string Title, string Description, string DetailedDescription, decimal Price, bool FeaturedArea, int DiscountRate, int TaxRate, bool RequiresPrepayment)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Update];
}

public class UpdateServiceHandler(IServiceRepository repository) : IRequestHandler<UpdateServiceCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await repository.GetAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);
        if (service is null)
            return ServiceBusinessRules.ServiceNotFound(request.Id);

        if (!string.Equals(service.Title, request.Title, StringComparison.Ordinal))
        {
            service.Slug = await SlugGenerator.GenerateUniqueAsync(request.Title,
                candidate => repository.AnyAsync(s => s.Slug == candidate && s.Id != request.Id, cancellationToken: cancellationToken));
        }

        service.CategoryId = request.CategoryId;
        service.Title = request.Title;
        service.Description = request.Description;
        service.DetailedDescription = request.DetailedDescription;
        service.Price = request.Price;
        service.FeaturedArea = request.FeaturedArea;
        service.DiscountRate = request.DiscountRate;
        service.TaxRate = request.TaxRate;
        service.DiscountedPrice = ServicePriceCalculator.CalculateDiscountedPrice(request.Price, request.DiscountRate);
        service.RequiresPrepayment = request.RequiresPrepayment;

        await repository.UpdateAsync(service);

        return Result.Success("Service updated successfully.");
    }
}
