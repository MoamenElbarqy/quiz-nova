using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed class GetAllQuizzesQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllQuizzesQuery, Result<List<QuizDto>>>
{
    public async Task<Result<List<QuizDto>>> Handle(GetAllQuizzesQuery request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var quizzes = await dbContext.Quizzes
            .AsNoTracking()
            .Select(quiz => new QuizDto(
                quiz.Id,
                quiz.Title,
                dbContext.Courses.Where(course => course.Id == quiz.CourseId)
                .Select(course => course.Name)
                .FirstOrDefault() ?? string.Empty,
                dbContext.Instructors
                .Where(instructor => instructor.Id == quiz.InstructorId)
                .Select(instructor => instructor.PersonalInformation.Name)
                .FirstOrDefault() ?? string.Empty,
                quiz.Questions.Sum(question => question.Marks),
                quiz.StartsAtUtc,
                quiz.EndsAtUtc,
                quiz.StartsAtUtc > now ? "Upcoming" : quiz.EndsAtUtc < now ? "Completed" : "Active"))
            .OrderBy(quiz => quiz.StartsAtUtc)
            .ToListAsync(ct);

        return quizzes;
    }
}
