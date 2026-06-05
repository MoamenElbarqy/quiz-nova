using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed record UpdateStudentCommand(
    Guid Id,
    PersonalInformationDto PersonalInformation)
    : IRequest<Result<StudentDto>>;
