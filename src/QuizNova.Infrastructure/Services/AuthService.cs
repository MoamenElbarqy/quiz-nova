using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users;
using QuizNova.Infrastructure.Data;
using QuizNova.Infrastructure.Settings;

using static System.Security.Claims.ClaimTypes;

namespace QuizNova.Infrastructure.Services;

public sealed class AuthService(AppDbContext dbContext, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private const int DefaultAccessTokenExpiryInMinutes = 7;
    private const int DefaultRefreshTokenExpiryInDays = 7;

    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct)
    {
        if (!Guid.TryParse(user.UserId, out var userId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;
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
        var accessTokenExpiresOnUtc = DateTime.UtcNow.AddMinutes(accessTokenExpiryInMinutes);

        var securityToken = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
            audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
            claims: claims,
            expires: accessTokenExpiresOnUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        var refreshTokenValue = GenerateSecureRefreshToken();
        var refreshTokenExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(refreshTokenExpiryInDays);

        var refreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            refreshTokenValue,
            userId,
            refreshTokenExpiresOnUtc);

        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.TopError;
        }

        await dbContext.RefreshTokens.AddAsync(refreshTokenResult.Value, ct);
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
        var audience = _jwtSettings.Audience;
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
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,
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

    public async Task<Result<UserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PersonalInformation.Email == email);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        if (!string.Equals(user.PersonalInformation.Password, password, StringComparison.Ordinal))
        {
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Invalid email or password.");
        }

        return MapUserDto(user);
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == parsedUserId);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        return MapUserDto(user);
    }

    public async Task<Result<Success>> ValidateAndRevokeRefreshTokenAsync(
        string userId,
        string refreshToken,
        CancellationToken ct)
    {
        if (!Guid.TryParse(userId, out var parsedUserId) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var storedRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == parsedUserId, ct);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var revokeResult = storedRefreshToken.Revoke(DateTimeOffset.UtcNow);
        if (revokeResult.IsError)
        {
            return revokeResult.Errors;
        }

        await dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }

    private static IList<Claim> BuildClaims(UserDto user)
    {
        var additionalClaims = user.Claims
            .Where(c => c.Type is not (NameIdentifier or Name or Role))
            .ToList();

        var claims = new List<Claim>(additionalClaims.Count + 3)
        {
            new(NameIdentifier, user.UserId),
            new(Name, user.Name),
            new(ClaimTypes.Role, user.Role),
        };

        claims.AddRange(additionalClaims);
        return claims;
    }

    private static string GenerateSecureRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private static UserDto MapUserDto(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(NameIdentifier, user.Id.ToString()),
            new Claim(Name, user.PersonalInformation.Name),
            new Claim(ClaimTypes.Role, user.UserRole.ToString()),
        };

        return new UserDto(
            user.Id.ToString(),
            user.PersonalInformation.Name,
            user.UserRole.ToString(),
            claims);
    }
}
