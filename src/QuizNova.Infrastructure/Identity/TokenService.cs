using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Infrastructure.Data;
using QuizNova.Infrastructure.Settings;

namespace QuizNova.Infrastructure.Identity;

public sealed class TokenService(
    AppDbContext dbContext,
    IOptions<JwtSettings> jwtOptions,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor)
    : ITokenService
{
    private const int DefaultAccessTokenExpiryInMinutes = 7;
    private const int DefaultRefreshTokenExpiryInDays = 7;

    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct)
    {
        var issuer = _jwtSettings.Issuer;
        var audience = GetValidAudienceFromRequest();
        var secret = _jwtSettings.Secret;
        if (string.IsNullOrWhiteSpace(secret))
        {
            return ApplicationErrors.TokenGenerationFailed;
        }

        var accessTokenExpiryInMinutes = _jwtSettings.ExpiryMinutes > 0
            ? _jwtSettings.ExpiryMinutes
            : DefaultAccessTokenExpiryInMinutes;
        var refreshTokenExpiryInDays = DefaultRefreshTokenExpiryInDays;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = BuildClaims(user);
        var accessTokenExpiresOnUtc = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(accessTokenExpiryInMinutes);

        var securityToken = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
            audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
            claims: claims,
            expires: accessTokenExpiresOnUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        var refreshTokenValue = GenerateSecureRefreshToken();
        var refreshTokenExpiresOnUtc = timeProvider.GetUtcNow().AddDays(refreshTokenExpiryInDays);

        var userRefreshToken = new UserRefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresOnUtc = refreshTokenExpiresOnUtc,
        };

        await dbContext.UserRefreshTokens.AddAsync(userRefreshToken, ct);
        await dbContext.SaveChangesAsync(ct);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresOnUtc = accessTokenExpiresOnUtc,
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var issuer = _jwtSettings.Issuer;
        var secret = _jwtSettings.Secret;

        if (string.IsNullOrWhiteSpace(secret))
        {
            return null;
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,
            ValidateAudience = _jwtSettings.Audiences.Length > 0,
            ValidAudiences = _jwtSettings.Audiences,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static IList<Claim> BuildClaims(UserDto user)
    {
        return new List<Claim>(3)
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role),
        };
    }

    private static string GenerateSecureRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private string? GetValidAudienceFromRequest()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var origin = httpContext.Request.Headers.Origin.ToString();
            if (!string.IsNullOrWhiteSpace(origin) && _jwtSettings.Audiences.Contains(origin))
            {
                return origin;
            }

            var referer = httpContext.Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                foreach (var aud in _jwtSettings.Audiences)
                {
                    if (referer.StartsWith(aud, StringComparison.OrdinalIgnoreCase))
                    {
                        return aud;
                    }
                }
            }
        }

        return _jwtSettings.Audiences.Length > 0 ? _jwtSettings.Audiences[0] : null;
    }
}
