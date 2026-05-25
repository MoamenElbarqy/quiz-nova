using QuizNova.Application.Common.Interfaces;
using QuizNova.Infrastructure.Identity;

namespace QuizNova.Tests.Common.Security;

public class TestCurrentUser : IUser
{
    private AppUser? _currentUser;

    public string? Id => _currentUser?.Id;

    public void Returns(AppUser currentUser)
    {
        _currentUser = currentUser;
    }
}
