using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(string? UserId) : ICachedQuery<Result<UserDto>>
{
    public string CacheKey => $"users:{UserId}";

    public string[] Tags => ["users"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
