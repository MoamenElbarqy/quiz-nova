using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users.UserPersonalInformation;

public static class PersonalInformationErrors
{
    public static readonly Error Required =
        Error.Validation("PersonalInformation_Required", "Personal information is required.");

    public static readonly Error NameRequired =
        Error.Validation("PersonalInformation_Name_Required", "Name is required.");

    public static readonly Error EmailRequired =
        Error.Validation("PersonalInformation_Email_Required", "Email is required.");

    public static readonly Error EmailInvalid =
        Error.Validation("PersonalInformation_Email_Invalid", "Email format is invalid.");

    public static readonly Error PasswordRequired =
        Error.Validation("PersonalInformation_Password_Required", "Password is required.");

    public static readonly Error PhoneNumberRequired =
        Error.Validation("PersonalInformation_PhoneNumber_Required", "Phone number is required.");

    public static readonly Error RoleInvalid =
        Error.Validation("PersonalInformation_Role_Invalid", "Role value is invalid.");
}
