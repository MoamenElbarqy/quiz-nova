using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admins.Queries.GetAllAdmins;

public sealed record GetAllAdminsQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10)
    : ICachedQuery<Result<PaginatedList<AdminDto>>>
{
    public string CacheKey => $"admins:all:{SearchTerm}:{PageNumber}:{PageSize}";

    public string[] Tags => ["admins"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
