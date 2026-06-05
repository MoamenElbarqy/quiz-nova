using MediatR;

using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed record CreateStudentCommand(
    PersonalInformationDto PersonalInformation,
    string Password,
    string Role)
    : IRequest<Result<StudentDto>>;
