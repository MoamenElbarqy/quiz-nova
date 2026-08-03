using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Driver;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Infrastructure.Settings;

namespace QuizNova.Infrastructure.Identity;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    IMongoDbContext mongoContext,
    IOptions<JwtSettings> jwtOptions,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthService> logger)
    : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result<AuthDto>> LoginAsync(
        string email,
        string password,
        string requestedRole,
        CancellationToken ct)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(appUser, password);
        if (!isPasswordValid)
        {
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Invalid email or password.");
        }

        var roles = await userManager.GetRolesAsync(appUser);
        var actualRole = roles.FirstOrDefault() ?? "Student";

        if (!string.Equals(actualRole, requestedRole, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "Login failed: User {Email} has role {ActualRole} but attempted to login as {RequestedRole}",
                email, actualRole, requestedRole);
            return ApplicationErrors.InvalidRoleForLogin;
        }

        var name = await GetUserNameAsync(appUser.Id);
        var userDto = new UserDto(appUser.Id, name, actualRole);

        var tokenResult = await GenerateJwtTokenAsync(userDto, ct);
        if (tokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", tokenResult.TopError.Description);
            return tokenResult.Errors;
        }

        return new AuthDto(tokenResult.Value, userDto);
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(
        string expiredAccessToken,
        string refreshToken,
        CancellationToken ct)
    {
        var principal = GetPrincipalFromExpiredToken(expiredAccessToken);
        if (principal is null)
        {
            logger.LogError("Expired access token is not valid");
            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            logger.LogError("Invalid userId claim");
            return ApplicationErrors.UserIdClaimInvalid;
        }

        var getUserResult = await GetUserByIdAsync(userId);
        if (getUserResult.IsError)
        {
            logger.LogError("Get user by id error occurred: {ErrorDescription}", getUserResult.TopError.Description);
            return getUserResult.Errors;
        }

        var validateRefreshTokenResult = await ValidateAndRevokeRefreshTokenAsync(userId, refreshToken, ct);
        if (validateRefreshTokenResult.IsError)
        {
            return validateRefreshTokenResult.Errors;
        }

        var tokenResult = await GenerateJwtTokenAsync(getUserResult.Value, ct);
        if (tokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", tokenResult.TopError.Description);
            return tokenResult.Errors;
        }

        return tokenResult.Value;
    }

    public async Task<Result<string>> RegisterUserAsync(
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return ApplicationErrors.UserEmailAlreadyExists(email);
        }

        var appUser = new AppUser
        {
            UserName = email,
            Email = email,
        };

        var result = await userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Error.Failure("Identity.UserCreationFailed", error.Description);
        }

        var roleResult = await userManager.AddToRoleAsync(appUser, role);
        if (!roleResult.Succeeded)
        {
            var error = roleResult.Errors.First();
            return Error.Failure("Identity.RoleAssignmentFailed", error.Description);
        }

        return appUser.Id;
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
    {
        var appUser = await userManager.FindByIdAsync(userId);
        if (appUser is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var roles = await userManager.GetRolesAsync(appUser);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var name = await GetUserNameAsync(appUser.Id);

        return new UserDto(appUser.Id, name, role);
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedGuid))
        {
            return string.Empty;
        }

        var user = await mongoContext.Users
            .Find(u => u.Id == parsedGuid)
            .FirstOrDefaultAsync();

        var name = user?.PersonalInformation.Name;

        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var appUser = await userManager.FindByIdAsync(userId);
        return appUser?.Email ?? string.Empty;
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

    private async Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken ct)
    {
        var issuer = _jwtSettings.Issuer;
        var audience = GetValidAudienceFromRequest();
        var secret = _jwtSettings.Secret;
        if (string.IsNullOrWhiteSpace(secret))
        {
            return ApplicationErrors.TokenGenerationFailed;
        }

        var accessTokenExpiryInMinutes = _jwtSettings.ExpiryMinutes;
        var refreshTokenExpiryInDays = _jwtSettings.RefreshTokenExpirationDays;

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

        await mongoContext.UserRefreshTokens.InsertOneAsync(userRefreshToken, cancellationToken: ct);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresOnUtc = accessTokenExpiresOnUtc,
        };
    }

    private async Task<Result<Success>> ValidateAndRevokeRefreshTokenAsync(
        string userId,
        string refreshToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var storedRefreshToken = await mongoContext.UserRefreshTokens
            .Find(rt => rt.Token == refreshToken && rt.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (storedRefreshToken is null)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        if (!storedRefreshToken.IsActive)
        {
            return ApplicationErrors.ExpiredOrRevokedRefreshToken;
        }

        storedRefreshToken.RevokedOnUtc = timeProvider.GetUtcNow();
        var filter = Builders<UserRefreshToken>.Filter.Eq(t => t.Id, storedRefreshToken.Id);
        var update = Builders<UserRefreshToken>.Update.Set(t => t.RevokedOnUtc, storedRefreshToken.RevokedOnUtc);
        await mongoContext.UserRefreshTokens.UpdateOneAsync(filter, update, cancellationToken: ct);

        return Result.Success;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
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
