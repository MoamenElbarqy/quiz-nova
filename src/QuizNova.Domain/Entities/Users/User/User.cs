using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users;
namespace QuizNova.Domain.Entities.Users;

public class User : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Password { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public Role Role { get; private set; }

    public List<RefreshToken> RefreshTokens { get; private set; } = [];

    protected User()
    {
    }

    protected User(Guid id, string name, string email, string password, string phoneNumber, Role role)
        : base(id)
    {
        Name = name;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public static Result<User> Create(Guid id,
                                      string name,
                                      string email,
                                      string password,
                                      string phoneNumber,
                                      Role role)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return UserErrors.EmailRequired;
        }

        if (!email.Contains('@'))
        {
            return UserErrors.EmailInvalid;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return UserErrors.PasswordRequired;
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return UserErrors.PhoneNumberRequired;
        }

        if (!Enum.IsDefined(role))
        {
            return UserErrors.RoleInvalid;
        }

        return new User(id, name, email, password, phoneNumber, role);
    }
}
