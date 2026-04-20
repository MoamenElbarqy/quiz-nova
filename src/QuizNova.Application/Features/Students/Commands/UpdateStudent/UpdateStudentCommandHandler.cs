using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken ct)
    {
        var student = await dbContext.Students
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (student is null)
        {
            return ApplicationErrors.StudentNotFound(request.Id);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.Id != request.Id && user.PersonalInformation.Email == request.Email, ct))
        {
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PhoneNumber,
                    ct))
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

        var updateStudentResult = student.Update(personalInformationResult.Value);

        if (updateStudentResult.IsError)
        {
            return updateStudentResult.TopError;
        }

        dbContext.Students.Update(student);
        await dbContext.SaveChangesAsync(ct);

        var enrolledCourseCount = await dbContext.StudentCourses
            .CountAsync(studentCourse => studentCourse.StudentId == student.Id, ct);

        return new StudentDto(
            student.Id,
            student.PersonalInformation.Name,
            student.PersonalInformation.Email,
            student.PersonalInformation.Password,
            student.PersonalInformation.PhoneNumber,
            enrolledCourseCount);
    }
}
