using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

using MediatR;

namespace QuizNova.Application.Features.Identity.Queries.GetUserInfo;

public sealed record GetUserByIdQuery(string? UserId) : IRequest<Result<AppUserDto>>;