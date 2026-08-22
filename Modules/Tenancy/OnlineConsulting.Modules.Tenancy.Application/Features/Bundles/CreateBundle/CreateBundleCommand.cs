using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.CreateBundle;

public record CreateBundleCommand(string Name, List<string> ModuleKeys, bool IsPubliclyVisible) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class CreateBundleHandler(IBundleRepository repository, IModuleOfferingRepository moduleOfferingRepository)
    : IRequestHandler<CreateBundleCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateBundleCommand request, CancellationToken cancellationToken)
    {
        var moduleKeys = request.ModuleKeys.Distinct().ToList();

        var existingKeys = (await moduleOfferingRepository.GetListAsync(
            m => moduleKeys.Contains(m.Key), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken))
            .Items.Select(m => m.Key).ToHashSet();

        var unknownKeys = moduleKeys.Where(k => !existingKeys.Contains(k)).ToList();
        if (unknownKeys.Count > 0)
        {
            return Result.BadRequest<Guid>(string.Format(BundleMessages.UnknownModuleKeysFormat, string.Join(", ", unknownKeys)));
        }

        var bundle = new Bundle
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ModuleKeys = moduleKeys,
            IsPubliclyVisible = request.IsPubliclyVisible,
        };

        _ = await repository.AddAsync(bundle);

        return Result.Created(bundle.Id, "Bundle created successfully.");
    }
}
