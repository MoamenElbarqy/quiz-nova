using MediatR;

using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(string? UserId) : IRequest<Result<UserDto>>;
