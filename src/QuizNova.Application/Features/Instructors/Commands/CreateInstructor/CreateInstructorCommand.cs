using MediatR;

using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Commands.CreateInstructor;

public sealed record CreateInstructorCommand(
    PersonalInformationDto PersonalInformation,
    string Password,
    string Role)
    : IRequest<Result<InstructorDto>>;
