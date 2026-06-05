using MediatR;

using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

public sealed record UpdateInstructorCommand(
    Guid Id,
    PersonalInformationDto PersonalInformation)
    : IRequest<Result<InstructorDto>>;
