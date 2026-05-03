using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetQuizByIdQueryHandler> logger)
    : IRequestHandler<GetQuizByIdQuery, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz details for ID: {QuizId}", request.QuizId);

        var quiz = await dbContext.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Include(q => q.Questions.OfType<Mcq>())
            .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

        if (quiz is null)
        {
            logger.LogWarning("Retrieval failed: Quiz with ID {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        logger.LogInformation("Successfully retrieved details for quiz {QuizId}", request.QuizId);

        var courseName = await dbContext.Courses
            .Where(course => course.Id == quiz.CourseId)
            .Select(course => course.Name)
            .FirstOrDefaultAsync(ct) ?? string.Empty;

        var instructorName = await dbContext.Instructors
            .Where(instructor => instructor.Id == quiz.InstructorId)
            .Select(instructor => instructor.PersonalInformation.Name)
            .FirstOrDefaultAsync(ct) ?? string.Empty;

        var dto = quiz.ToQuizDto();
        return new QuizDto
        {
            QuizId = dto.QuizId,
            Title = dto.Title,
            CourseName = courseName,
            InstructorName = instructorName,
            Marks = dto.Marks,
            StartsAtUtc = dto.StartsAtUtc,
            EndsAtUtc = dto.EndsAtUtc,
            ServerUtc = dto.ServerUtc,
            State = dto.State,
            CourseId = dto.CourseId,
            InstructorId = dto.InstructorId,
            Questions = dto.Questions,
        };
    }
}
