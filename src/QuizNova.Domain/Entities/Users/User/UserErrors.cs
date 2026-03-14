using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users;

public static class UserErrors
{
    public static readonly Error NameRequired =
        Error.Validation("User_Name_Required", "Name is required.");

    public static readonly Error EmailRequired =
        Error.Validation("User_Email_Required", "Email is required.");

    public static readonly Error EmailInvalid =
        Error.Validation("User_Email_Invalid", "Email format is invalid.");

    public static readonly Error PasswordRequired =
        Error.Validation("User_Password_Required", "Password is required.");

    public static readonly Error PhoneNumberRequired =
        Error.Validation("User_PhoneNumber_Required", "Phone number is required.");

    public static readonly Error RoleInvalid =
        Error.Validation("User_Role_Invalid", "Role value is invalid.");
}
