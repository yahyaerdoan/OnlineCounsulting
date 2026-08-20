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

    public async Task<OperationDataResult<string>> Handle(GetOrCreateReferralCodeCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetAsync(c => c.UserId == request.UserId, cancellationToken: cancellationToken);
        if (existing is not null)
        {
            return Result.Success(existing.Code, "Referral code retrieved successfully.");
        }

        string code;
        do
        {
            code = GenerateCode();
        }
        while (await repository.AnyAsync(c => c.Code == code, cancellationToken: cancellationToken));

        _ = await repository.AddAsync(new ReferralCode { Id = Guid.NewGuid(), UserId = request.UserId, Code = code });

        return Result.Created(code, "Referral code created successfully.");
    }

    private static string GenerateCode() => string.Create(8, 0, (span, _) =>
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }
    });
}
