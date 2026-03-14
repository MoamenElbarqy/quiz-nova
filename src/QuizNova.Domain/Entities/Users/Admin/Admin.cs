using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Security;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class Admin : User
{
    private readonly List<Permission> _permissions;

    private readonly List<Instructor> _instructors;

    private Admin(
        Guid id,
        PersonalInformation personalInformation,
        Guid collegeId,
        List<RefreshToken> refreshTokens,
        List<Permission> permissions,
        List<Instructor> instructors)
        : base(
            id,
            personalInformation,
            Role.Admin,
            refreshTokens)
    {
        CollegeId = collegeId;
        _permissions = permissions;
        _instructors = instructors;
    }

    public Guid CollegeId { get; private set; }

    public College? College { get; private set; }

    public IEnumerable<Permission> Permissions => _permissions.AsReadOnly();

    public IEnumerable<Instructor> Instructors => _instructors.AsReadOnly();

    public static Result<Admin> Create(
        Guid id,
        PersonalInformation personalInformation,
        Guid collegeId,
        List<RefreshToken> refreshTokens,
        List<Permission> permissions,
        List<Instructor> instructors)
    {
        if (collegeId == Guid.Empty)
        {
            return AdminErrors.CollegeIdRequired;
        }

        var validationError = ValidateCommon(personalInformation, Role.Admin);

        if (validationError is not null)
        {
            return validationError;
        }

        return new Admin(id, personalInformation, collegeId, refreshTokens, permissions, instructors);
    }
}
