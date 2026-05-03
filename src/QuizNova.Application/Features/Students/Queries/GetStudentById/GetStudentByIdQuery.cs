using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetStudentById;

public sealed record GetStudentByIdQuery(Guid Id)
    : ICachedQuery<Result<StudentDto>>
{
    public string CacheKey => $"students:{Id}";

    public string[] Tags => ["students"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
