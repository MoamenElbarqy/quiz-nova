using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Infrastructure.Data;

namespace QuizNova.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<AppUser> userManager,
    AppDbContext dbContext)
    : IIdentityService
{
    public async Task<Result<UserDto>> AuthenticateAsync(string email, string password)
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
        var role = roles.FirstOrDefault() ?? "Student";

        var name = await GetUserNameAsync(appUser.Id);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, appUser.Id),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role),
        };

        return new UserDto(appUser.Id, name, role, claims);
    }

    public async Task<Result<string>> RegisterUserAsync(
        string email,
        string password,
        string name,
        string role,
        CancellationToken ct)
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

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, appUser.Id),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role),
        };

        return new UserDto(appUser.Id, name, role, claims);
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedGuid))
        {
            return string.Empty;
        }

        var name = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == parsedGuid)
            .Select(u => u.PersonalInformation.Name)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var appUser = await userManager.FindByIdAsync(userId);
        return appUser?.Email ?? string.Empty;
    }

    public async Task<Result<Success>> ValidateAndRevokeRefreshTokenAsync(
        string userId,
        string refreshToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        var storedRefreshToken = await dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId, ct);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return ApplicationErrors.InvalidRefreshToken;
        }

        storedRefreshToken.RevokedOnUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }
}
