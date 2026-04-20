using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Instructor.Commands.CreateInstructor;

public sealed class CreateInstructorCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateInstructorCommand, Result<InstructorDto>>
{
    public async Task<Result<InstructorDto>> Handle(CreateInstructorCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Instructor)
        {
            return ApplicationErrors.CreateInstructorRoleInvalid(request.Role);
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

        var createInstructorResult = QuizNova.Domain.Entities.Users.Instructor.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>(),
            new List<Domain.Entities.Courses.Course>(),
            new List<Domain.Entities.Quizzes.Quiz>());

        if (createInstructorResult.IsError)
        {
            return createInstructorResult.TopError;
        }

        await dbContext.Instructors.AddAsync(createInstructorResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        return new InstructorDto(
            createInstructorResult.Value.Id,
            createInstructorResult.Value.PersonalInformation.Name,
            createInstructorResult.Value.PersonalInformation.Email,
            createInstructorResult.Value.PersonalInformation.Password,
            createInstructorResult.Value.PersonalInformation.PhoneNumber,
            0,
            0);
    }
}
