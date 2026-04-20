using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructor.Commands.UpdateInstructor;

public sealed class UpdateInstructorCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken ct)
    {
        var instructor = await dbContext.Instructors
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (instructor is null)
        {
            return ApplicationErrors.InstructorNotFound(request.Id);
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

        var updateInstructorResult = instructor.Update(personalInformationResult.Value);

        if (updateInstructorResult.IsError)
        {
            return updateInstructorResult.TopError;
        }

        dbContext.Instructors.Update(instructor);
        await dbContext.SaveChangesAsync(ct);

        var courseCount = await dbContext.Courses
            .CountAsync(course => course.InstructorId == instructor.Id, ct);

        var quizCount = await dbContext.Quizzes
            .CountAsync(quiz => quiz.InstructorId == instructor.Id, ct);

        return new InstructorDto(
            instructor.Id,
            instructor.PersonalInformation.Name,
            instructor.PersonalInformation.Email,
            instructor.PersonalInformation.Password,
            instructor.PersonalInformation.PhoneNumber,
            courseCount,
            quizCount);
    }
}
