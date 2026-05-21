using Microsoft.AspNetCore.Identity;

namespace QuizNova.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public List<UserRefreshToken> RefreshTokens { get; set; } = [];
}
