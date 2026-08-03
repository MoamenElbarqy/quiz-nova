namespace QuizNova.Domain.Entities.Identity;

public class UserRefreshToken
{
    public Guid Id { get; init; }

    public string Token { get; init; }

    public string UserId { get; init; }

    public DateTimeOffset ExpiresOnUtc { get; init; }

    public DateTimeOffset? RevokedOnUtc { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public bool IsActive => RevokedOnUtc == null && !IsExpired;
}
