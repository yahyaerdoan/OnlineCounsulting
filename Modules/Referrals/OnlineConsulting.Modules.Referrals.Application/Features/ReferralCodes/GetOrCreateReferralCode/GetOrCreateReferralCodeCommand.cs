using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;
using OnlineConsulting.Modules.Referrals.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.GetOrCreateReferralCode;

/// <summary>UserId is always resolved server-side from the authenticated caller, never trusted from the client.</summary>
public record GetOrCreateReferralCodeCommand(Guid UserId) : IRequest<OperationDataResult<string>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class GetOrCreateReferralCodeHandler(IReferralCodeRepository repository) : IRequestHandler<GetOrCreateReferralCodeCommand, OperationDataResult<string>>
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;
    private const int MaxGenerationAttempts = 10;

    public async Task<OperationDataResult<string>> Handle(GetOrCreateReferralCodeCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetAsync(c => c.UserId == request.UserId, cancellationToken: cancellationToken);
        if (existing is not null)
        {
            return Result.Success(existing.Code, "Referral code retrieved successfully.");
        }

        var code = await GenerateUniqueCodeAsync(cancellationToken);
        if (code is null)
        {
            return Result.InternalServerError<string>("Could not generate a unique referral code. Please try again.");
        }

        _ = await repository.AddAsync(new ReferralCode { Id = Guid.NewGuid(), UserId = request.UserId, Code = code });

        return Result.Created(code, "Referral code created successfully.");
    }

    private async Task<string?> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxGenerationAttempts; attempt++)
        {
            var code = GenerateCode();
            if (!await repository.AnyAsync(c => c.Code == code, cancellationToken: cancellationToken))
            {
                return code;
            }
        }

        return null;
    }

    private static string GenerateCode() => string.Create(CodeLength, 0, (span, _) =>
    {
        foreach (ref var c in span)
        {
            c = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }
    });
}
