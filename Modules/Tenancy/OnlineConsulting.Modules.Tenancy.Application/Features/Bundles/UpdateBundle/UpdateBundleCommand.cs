using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.UpdateBundle;

/// <summary>Unlike ModuleOffering/MembershipPlan, Bundle has no provider-side price of its own (see Bundle doc comment) - every field is freely editable.</summary>
public record UpdateBundleCommand(Guid Id, string Name, List<string> ModuleKeys, bool IsPubliclyVisible) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class UpdateBundleHandler(IBundleRepository repository, IModuleOfferingRepository moduleOfferingRepository)
    : IRequestHandler<UpdateBundleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateBundleCommand request, CancellationToken cancellationToken)
    {
        var bundle = await repository.GetAsync(b => b.Id == request.Id, cancellationToken: cancellationToken);
        if (bundle is null)
        {
            return Result.NotFound(string.Format(BundleMessages.BundleNotFoundFormat, request.Id));
        }

        var moduleKeys = request.ModuleKeys.Distinct().ToList();

        var existingKeys = (await moduleOfferingRepository.GetListAsync(
            m => moduleKeys.Contains(m.Key), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken))
            .Items.Select(m => m.Key).ToHashSet();

        var unknownKeys = moduleKeys.Where(k => !existingKeys.Contains(k)).ToList();
        if (unknownKeys.Count > 0)
        {
            return Result.BadRequest(string.Format(BundleMessages.UnknownModuleKeysFormat, string.Join(", ", unknownKeys)));
        }

        bundle.Name = request.Name;
        bundle.ModuleKeys = moduleKeys;
        bundle.IsPubliclyVisible = request.IsPubliclyVisible;

        _ = await repository.UpdateAsync(bundle);

        return Result.Success("Bundle updated successfully.");
    }
}
