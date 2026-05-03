using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructors.Queries.GetInstructorById;

public sealed record GetInstructorByIdQuery(Guid Id)
    : ICachedQuery<Result<InstructorDto>>
{
    public string CacheKey => $"instructors:{Id}";

    public string[] Tags => ["instructors"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
