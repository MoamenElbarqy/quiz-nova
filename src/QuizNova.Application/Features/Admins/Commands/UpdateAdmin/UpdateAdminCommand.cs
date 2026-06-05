using MediatR;

using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

public sealed record UpdateAdminCommand(
    Guid Id,
    PersonalInformationDto PersonalInformation)
    : IRequest<Result<AdminDto>>;
