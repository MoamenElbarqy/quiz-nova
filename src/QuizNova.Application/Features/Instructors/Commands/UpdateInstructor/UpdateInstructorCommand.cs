using MediatR;

using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

public sealed record UpdateInstructorCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber)
    : IRequest<Result<InstructorDto>>;
