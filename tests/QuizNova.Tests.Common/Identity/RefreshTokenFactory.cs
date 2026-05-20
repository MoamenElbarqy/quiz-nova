using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Tests.Common.Identity;

public static class RefreshTokenFactory
{
    public static Result<RefreshToken> CreateRefreshToken(
        Guid? id = null,
        string token = "test-refresh-token-123456",
        Guid? userId = null,
        DateTimeOffset? expiresOnUtc = null)
    {
        return RefreshToken.Create(
            id ?? Guid.NewGuid(),
            token,
            userId ?? Guid.NewGuid(),
            expiresOnUtc ?? DateTimeOffset.UtcNow.AddDays(7));
    }
}
