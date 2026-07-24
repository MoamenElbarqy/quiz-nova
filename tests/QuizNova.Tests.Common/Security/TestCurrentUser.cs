using QuizNova.Application.Common.Interfaces;
using QuizNova.Infrastructure.Identity;

namespace QuizNova.Tests.Common.Security;

public class TestCurrentUser : IUser
{
    private static readonly AsyncLocal<AppUser?> CurrentUserHolder = new();

    public string? Id => CurrentUserHolder.Value?.Id;

    public static void Set(AppUser? currentUser)
    {
        CurrentUserHolder.Value = currentUser;
    }
}
