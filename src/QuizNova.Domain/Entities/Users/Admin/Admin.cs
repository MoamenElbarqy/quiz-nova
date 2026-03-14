using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities..Colleges;
using QuizNova.Domain.Entities.Security;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Domain.Entities.Users;

public class Admin : User
{
    public Guid CollegeId { get; private set; }

    public College? College { get; private set; }

    public List<Permission> Permissions { get; private set; } = [];

    public List<Instructor> Instructors { get; private set; } = [];

    private Admin()
    {
    }

    private Admin(User user, Guid collegeId) : base(user.Id,
                                                    user.Name,
                                                    user.Email,
                                                    user.Password,
                                                    user.PhoneNumber,
                                                    Role.Admin)
    {
        CollegeId = collegeId;
    }

    public static Result<Admin> Create(Guid id, string name, string email, string password, string phoneNumber, Guid collegeId)
    {
        if (collegeId == Guid.Empty)
        {
            return AdminErrors.CollegeIdRequired;
        }

        var user = User.Create(id, name, email, password, phoneNumber, Role.Admin);

        if (user.IsError)
        {
            return user.Errors;
        }
        return new Admin(user.Value, collegeId);
    }
}
