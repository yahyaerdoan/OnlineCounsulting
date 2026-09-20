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
    private const string _alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int _codeLength = 8;
    private const int _maxGenerationAttempts = 10;

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

        _ = await repository.AddAsync(new ReferralCode { UserId = request.UserId, Code = code });

        return Result.Created(code, "Referral code created successfully.");
    }

    private async Task<string?> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < _maxGenerationAttempts; attempt++)
        {
            var code = GenerateCode();

            if (!await repository.AnyAsync(c => c.Code == code, cancellationToken: cancellationToken))
            {
                return code;
            }
        }

        return null;
    }

    private static string GenerateCode() => string.Create(_codeLength, 0, (span, _) =>
    {
        foreach (ref var c in span)
        {
            c = _alphabet[RandomNumberGenerator.GetInt32(_alphabet.Length)];
        }
    });
}
