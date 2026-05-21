using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.Admins;

public static class AdminFactory
{
    public static Result<Admin> Create(
        Guid? id = null,
        PersonalInformation? personalInformation = null)
    {
        if (personalInformation == null)
        {
            personalInformation = PersonalInformationFactory.CreatePersonalInformation(name: "Test Admin", email: "admin@example.com");
        }

        return Admin.Create(
            id ?? Guid.NewGuid(),
            personalInformation);
    }
}
