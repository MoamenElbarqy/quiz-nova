using Microsoft.AspNetCore.Identity;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public List<UserRefreshToken> RefreshTokens { get; set; } = [];
}
