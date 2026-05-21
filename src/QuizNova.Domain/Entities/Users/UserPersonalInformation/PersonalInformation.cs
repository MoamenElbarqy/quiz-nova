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
        string phoneNumber)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public static Result<PersonalInformation> Create(
        string name,
        string email,
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

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return PersonalInformationErrors.PhoneNumberRequired;
        }

        var trimmedPhone = phoneNumber.Trim();
        if (trimmedPhone.Length is < 7 or > 15)
        {
            return PersonalInformationErrors.PhoneNumberInvalid;
        }

        return new PersonalInformation(trimmedName, trimmedEmail, trimmedPhone);
    }
}
