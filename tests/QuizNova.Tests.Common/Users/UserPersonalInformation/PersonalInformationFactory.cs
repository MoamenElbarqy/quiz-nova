using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.UserPersonalInformation;

public static class PersonalInformationFactory
{
    public static PersonalInformation CreatePersonalInformation(
        string name = "Test User",
        string email = "test@example.com",
        string password = "SecurePassword123!",
        string phoneNumber = "1234567890")
    {
        return PersonalInformation.Create(name, email, password, phoneNumber).Value;
    }
}
