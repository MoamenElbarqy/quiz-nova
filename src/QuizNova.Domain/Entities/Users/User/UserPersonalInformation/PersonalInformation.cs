using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users.UserPersonalInformation;

public sealed class PersonalInformation
{
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

    public string Name { get; }

    public string Email { get; }

    public string Password { get; }

    public string PhoneNumber { get; }

    public static Result<PersonalInformation> Create(
        string name,
        string email,
        string password,
        string phoneNumber)
    {
        var validationError = Validate(name, email, password, phoneNumber);

        if (validationError is not null)
        {
            return validationError;
        }

        return new PersonalInformation(name, email, password, phoneNumber);
    }

    public static Error? Validate(PersonalInformation personalInformation)
    {
        return Validate(
            personalInformation.Name,
            personalInformation.Email,
            personalInformation.Password,
            personalInformation.PhoneNumber);
    }

    private static Error? Validate(
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

        return null;
    }
}
