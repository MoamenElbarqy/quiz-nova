using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Student)
        {
            return ApplicationErrors.CreateStudentRoleInvalid(request.Role);
        }

        if (await dbContext.Entities.OfType<User>().AnyAsync(user => user.Id == request.Id, ct))
        {
            return ApplicationErrors.UserIdAlreadyExists(request.Id);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.PersonalInformation.Email == request.Email, ct))
        {
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
        {
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            return personalInformationResult.TopError;
        }

        var createStudentResult = Student.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>(),
            new List<Domain.Entities.StudentCourses.StudentCourse>(),
            new List<Domain.Entities.QuizAttempts.QuizAttempt>());

        if (createStudentResult.IsError)
        {
            return createStudentResult.TopError;
        }

        await dbContext.Students.AddAsync(createStudentResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        return new StudentDto(
            createStudentResult.Value.Id,
            createStudentResult.Value.PersonalInformation.Name,
            createStudentResult.Value.PersonalInformation.Email,
            createStudentResult.Value.PersonalInformation.Password,
            createStudentResult.Value.PersonalInformation.PhoneNumber,
            0);
    }
}
