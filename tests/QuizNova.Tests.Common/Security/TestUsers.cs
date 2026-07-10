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
            Email = "ahmed.nasser@quiznova.local",
            UserName = "ahmed.nasser@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Instructor123!",
    };

    public static TestUserAccount Instructor2 => new()
    {
        User = new AppUser
        {
            Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            Email = "sara.kamel@quiznova.local",
            UserName = "sara.kamel@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Instructor123!",
    };

    public static TestUserAccount Instructor3 => new()
    {
        User = new AppUser
        {
            Id = "b2c3d4e5-f6a7-8901-bcde-f12345678901",
            Email = "marwan.hosny@quiznova.local",
            UserName = "marwan.hosny@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Instructor123!",
    };

    public static TestUserAccount Student => new()
    {
        User = new AppUser
        {
            Id = "b6327240-0aea-46fc-863a-777fc4e42560",
            Email = "omar.yasser@quiznova.local",
            UserName = "omar.yasser@quiznova.local",
            EmailConfirmed = true,
        },
        Password = "Student123!",
    };
}
