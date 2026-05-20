using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.Admins;

public static class AdminFactory
{
    public static Result<Admin> CreateAdmin(
        PersonalInformation? personalInformation = null,
        List<RefreshToken>? refreshTokens = null)
    {
        return Admin.Create(
            personalInformation ?? PersonalInformationFactory.CreatePersonalInformation(name: "Test Admin", email: "admin@example.com"),
            refreshTokens ?? []);
    }
}
