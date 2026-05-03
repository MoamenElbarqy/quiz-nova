using MediatR;

using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

public sealed record UpdateAdminCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber)
    : IRequest<Result<AdminDto>>;
