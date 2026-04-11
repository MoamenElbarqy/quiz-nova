using QuizNova.Domain.Common.Results;

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
        var validationError = Validate(name, email, password, phoneNumber);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new PersonalInformation(name, email, password, phoneNumber);
    }

    public static Result<Validated> Validate(PersonalInformation personalInformation)
    {
        return Validate(
            personalInformation.Name,
            personalInformation.Email,
            personalInformation.Password,
            personalInformation.PhoneNumber);
    }

    private static Result<Validated> Validate(
        string name,
        string email,
        string password,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return PersonalInformationErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return PersonalInformationErrors.EmailRequired;
        }

        if (!email.Contains('@'))
        {
            return PersonalInformationErrors.EmailInvalid;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return PersonalInformationErrors.PasswordRequired;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return PersonalInformationErrors.PhoneNumberRequired;
        }

        return Result.Validated;
    }
}
