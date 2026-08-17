using Core.SecurityLayer.Extensions;
using Core.SecurityLayer.JsonWebTokens.Abstractions;
using Core.SecurityLayer.JsonWebTokens.Concretions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Security;

/// <summary>Thin adapter over <see cref="IJwtTokenHelper"/> - builds the claim set, delegates actual token creation.</summary>
public class TokenManager(IJwtTokenHelper jwtTokenHelper, IOptions<TokenOption> tokenOption) : ITokenService
{
    private readonly TokenOption _tokenOption = tokenOption.Value;

    public (string Token, DateTime ExpiresAt) CreateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TenantProvider.TenantClaimType, user.TenantId.ToString()),
        };
        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddName(user.UserName ?? string.Empty);
        claims.AddEmail(user.Email ?? string.Empty);
        claims.AddRoles([.. roles]);
        claims.AddPermissions([.. permissions]);

        var accessToken = jwtTokenHelper.CreateToken(claims);
        return (accessToken.Token, accessToken.Expiration);
    }

    /// <summary>Reads claims from an expired token for the refresh flow - <see cref="IJwtTokenHelper"/> only creates tokens.</summary>
    public string? GetUserIdFromExpiredToken(string accessToken)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _tokenOption.Issuer,
            ValidAudience = _tokenOption.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.SecurityKey)),
            ValidateLifetime = false,
        };

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
                return null;

            // JwtSecurityTokenHandler remaps "sub" to NameIdentifier by default - same dual lookup as CurrentUserAccessor.
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}
