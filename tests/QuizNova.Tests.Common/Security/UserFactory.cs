using QuizNova.Infrastructure.Identity;

namespace QuizNova.Tests.Common.Security;

internal static class UserFactory
{
    public static AppUser CreateUser()
    {
        return new AppUser
        {
            Id = "19a59129-6c20-417a-834d-11a208d32d96",
            Email = "user@quiznova.local",
            UserName = "user@quiznova.local",
            EmailConfirmed = true,
        };
    }
}
