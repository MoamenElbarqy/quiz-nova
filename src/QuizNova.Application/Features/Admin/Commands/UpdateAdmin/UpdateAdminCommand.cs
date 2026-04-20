using MediatR;

using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Commands.UpdateAdmin;

public sealed record UpdateAdminCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string PhoneNumber)
    : IRequest<Result<AdminDto>>;
