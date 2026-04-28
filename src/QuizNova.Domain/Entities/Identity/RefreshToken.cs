using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Identity;

public sealed class RefreshToken : Entity
{
    public string Token { get; private set; } = string.Empty;

    public Guid UserId { get; private set; }

    public DateTimeOffset ExpiresOnUtc { get; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public bool IsActive => RevokedAtUtc is null && !IsExpired;

    public User? User { get; set; }

    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid id,
        string token,
        Guid userId,
        DateTimeOffset expiresOnUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public static Result<RefreshToken> Create(
        Guid id,
        string? token,
        Guid userId,
        DateTimeOffset expiresOnUtc)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (userId == Guid.Empty)
        {
            return RefreshTokenErrors.UserIdRequired;
        }

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return new RefreshToken(id, token, userId, expiresOnUtc);
    }

    public Result<Success> Revoke(DateTimeOffset revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
        {
            return RefreshTokenErrors.AlreadyRevoked;
        }

        RevokedAtUtc = revokedAtUtc;
        return Result.Success;
    }
}
