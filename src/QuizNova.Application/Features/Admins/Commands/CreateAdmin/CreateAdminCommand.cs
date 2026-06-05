using MediatR;

using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Commands.CreateAdmin;

public sealed record CreateAdminCommand(
    PersonalInformationDto PersonalInformation,
    string Password,
    string Role)
    : IRequest<Result<AdminDto>>;
