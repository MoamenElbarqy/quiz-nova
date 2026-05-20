using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Common.Utilities;

namespace QuizNova.Domain.Entities.Users.UserPersonalInformation;

public sealed class PersonalInformation
{
    private PersonalInformation()
    {
    }

    private PersonalInformation(
        string name,
        string email,
        string password,
        string phoneNumber)
    {
        Name = name;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
    }

    public string Name { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string Password { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    public static Result<PersonalInformation> Create(
        string name,
        string email,
        string password,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return PersonalInformationErrors.NameRequired;
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length < 3)
        {
            return PersonalInformationErrors.NameInvalid;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return PersonalInformationErrors.EmailRequired;
        }

        var trimmedEmail = email.Trim();
        if (!ValidationUtils.IsValidEmailFormat(trimmedEmail))
        {
            return PersonalInformationErrors.EmailInvalid;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return PersonalInformationErrors.PasswordRequired;
        }

        var trimmedPassword = password.Trim();
        if (trimmedPassword.Length < 8)
        {
            return PersonalInformationErrors.PasswordInvalid;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return PersonalInformationErrors.PhoneNumberRequired;
        }

        var trimmedPhone = phoneNumber.Trim();
        if (trimmedPhone.Length < 7 || trimmedPhone.Length > 15)
        {
            return PersonalInformationErrors.PhoneNumberInvalid;
        }

        return new PersonalInformation(trimmedName, trimmedEmail, trimmedPassword, trimmedPhone);
    }
}
