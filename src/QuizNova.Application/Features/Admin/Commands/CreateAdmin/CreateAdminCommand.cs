using MediatR;

using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Commands.CreateAdmin;

public sealed record CreateAdminCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber,
    string Role)
    : IRequest<Result<AdminDto>>;
