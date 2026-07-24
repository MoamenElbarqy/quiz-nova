using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    ILogger<GetQuizByIdQueryHandler> logger)
    : IRequestHandler<GetQuizByIdQuery, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving quiz details for ID: {QuizId}", request.QuizId);

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

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

        return quiz.ToQuizDto(courseName, instructorName);
    }
}

