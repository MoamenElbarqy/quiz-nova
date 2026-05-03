using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Queries.GetAdminById;

public sealed record GetAdminByIdQuery(Guid Id)
    : ICachedQuery<Result<AdminDto>>
{
    public string CacheKey => $"admins:{Id}";

    public string[] Tags => ["admins"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
