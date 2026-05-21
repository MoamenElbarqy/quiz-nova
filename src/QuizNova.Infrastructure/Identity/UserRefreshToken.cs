namespace QuizNova.Infrastructure.Identity;

public class UserRefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTimeOffset ExpiresOnUtc { get; set; }

    public DateTimeOffset? RevokedOnUtc { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public bool IsActive => RevokedOnUtc == null && !IsExpired;

    public AppUser User { get; set; } = null!;
}
