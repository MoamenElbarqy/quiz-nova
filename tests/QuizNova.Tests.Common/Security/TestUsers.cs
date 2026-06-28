using QuizNova.Infrastructure.Identity;

namespace QuizNova.Tests.Common.Security;

public class TestUserAccount
{
    public required AppUser User { get; init; }

    public required string Password { get; init; }
}

public static class TestUsers
{
    public static TestUserAccount Admin => new()
    {
        User = new AppUser
        {
            Id = "d7d11db8-0ce0-48b4-8ab3-7729cb4187f5",
            Email = "admin@quiznova.local",
            UserName = "admin@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Admin123!",
    };

    public static TestUserAccount Instructor1 => new()
    {
        User = new AppUser
        {
            Id = "54cd01ba-b9ae-4c14-bab6-f3df0219ba4c",
            Email = "instructor1@quiznova.local",
            UserName = "instructor1@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Instructor123!",
    };

    public static TestUserAccount Student => new()
    {
        User = new AppUser
        {
            Id = "b6327240-0aea-46fc-863a-777fc4e42560",
            Email = "student1@quiznova.local",
            UserName = "student1@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Student123!",
    };
}
