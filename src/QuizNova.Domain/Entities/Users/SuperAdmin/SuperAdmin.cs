using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities..Colleges;

namespace QuizNova.Domain.Entities.Users;

public class SuperAdmin : User
{
    public List<College> Colleges { get; private set; } = [];

    private SuperAdmin()
    {
    }

    private SuperAdmin(User user)
        : base(user.Id, user.Name, user.Email, user.Password, user.PhoneNumber, Role.SuperAdmin)
    {
    }

    public static Result<SuperAdmin> Create(Guid id, string name, string email, string password, string phoneNumber)
    {
        var user = User.Create(id, name, email, password, phoneNumber, Role.SuperAdmin);

        if (user.IsError)
        {
            return user.Errors;
        }

        return new SuperAdmin(user.Value);
    }
}
