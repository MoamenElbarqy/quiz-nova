using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Identity;

public static class RefreshTokenErrors
{
    public static readonly Error TokenRequired =
        Error.Validation("RefreshToken_Token_Required", "Token value is required.");

    public static readonly Error UserIdRequired =
        Error.Validation("RefreshToken_UserId_Required", "User ID is required.");

    public static readonly Error ExpiryInvalid =
        Error.Validation("RefreshToken_Expiry_Invalid", "Expiry must be in the future.");

    public static readonly Error AlreadyRevoked =
        Error.Conflict("RefreshToken_AlreadyRevoked", "Refresh token has already been revoked.");
}
