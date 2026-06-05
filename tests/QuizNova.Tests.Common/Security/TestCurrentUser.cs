using QuizNova.Application.Common.Interfaces;
using QuizNova.Infrastructure.Identity;

namespace QuizNova.Tests.Common.Security;

public class TestCurrentUser : IUser
{
    // async local like a local storage for async context so it preserve the context of the current user test context
    // to rsolve the problem of using raw AppUser and different tests manipluate them
    // this also make the runtime preserve the AppUser in the Execution Context of the thread
    // and whenever making await call the value is preserved
    private static readonly AsyncLocal<AppUser?> CurrentUserHolder = new();

    public string? Id => CurrentUserHolder.Value?.Id;

    public static void Set(AppUser? currentUser)
    {
        CurrentUserHolder.Value = currentUser;
    }
}
