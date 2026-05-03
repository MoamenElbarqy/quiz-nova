using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed record GetCollegeSummaryQuery() : ICachedQuery<Result<CollegeSummaryDto>>
{
    public string CacheKey => "colleges:summary";

    public string[] Tags => ["colleges"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
