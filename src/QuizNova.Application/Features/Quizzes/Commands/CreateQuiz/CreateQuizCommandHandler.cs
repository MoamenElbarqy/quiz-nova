using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler(
    IAppDbContext dbContext,
    IMongoDbContext mongoContext,
    IUser user,
    ILogger<CreateQuizCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(CreateQuizCommand request, CancellationToken ct)
    {
        var instructorId = Guid.Parse(user.Id!);

        logger.LogInformation(
            "Creating quiz with title: {Title} for course: {CourseId}",
            request.Title,
            request.CourseId);

        var quizId = Guid.NewGuid();

        var course = await dbContext.Courses
            .FirstOrDefaultAsync(course => course.Id == request.CourseId, ct);

        if (course is null)
        {
            logger.LogWarning("Quiz creation failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.QuizCourseNotFound(request.CourseId);
        }

        var instructor = await dbContext.Instructors
            .FirstOrDefaultAsync(instructor => instructor.Id == instructorId, ct);

        if (instructor is null)
        {
            logger.LogWarning("Quiz creation failed: Instructor {InstructorId} not found", instructorId);
            return ApplicationErrors.QuizInstructorNotFound(instructorId);
        }

        if (course.InstructorId != instructorId)
        {
            logger.LogWarning(
                "Quiz creation failed: Instructor {InstructorId} is not assigned to course {CourseId}",
                instructorId,
                request.CourseId);

            return ApplicationErrors.QuizInstructorIsNotAssignedToCourse(instructorId, request.CourseId);
        }

        if (course.Status == CourseStatus.Completed)
        {
            logger.LogWarning("Quiz creation failed: Course {CourseId} is completed", request.CourseId);
            return ApplicationErrors.QuizCourseCompleted(request.CourseId);
        }

        var questionArgs = request.Questions.Select(MapToDomainArgs).ToList();

        var createQuizResult = Quiz.Create(
            quizId,
            request.CourseId,
            instructorId,
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc,
            questionArgs);

        if (createQuizResult.IsError)
        {
            logger.LogWarning(
                "Quiz creation failed: Error creating quiz entity. Error: {ErrorDescription}",
                createQuizResult.TopError.Description);
            return createQuizResult.TopError;
        }

        var quiz = createQuizResult.Value;

        await mongoContext.Quizzes.InsertOneAsync(quiz, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Quizzes], ct);

        logger.LogInformation(
            "Successfully created quiz {QuizId} with {QuestionCount} questions",
            quizId,
            quiz.Questions.Count());

        return quiz.ToQuizDto(course.Name, instructor.PersonalInformation.Name);
    }

    private static CreateQuestionArgs MapToDomainArgs(CreateQuestionCommand command) => command switch
    {
        CreateTfCommand tf => new CreateTfArgs(tf.QuestionText, tf.Marks, tf.CorrectChoice),
        CreateMcqCommand mcq => new CreateMcqArgs(
            mcq.QuestionText,
            mcq.Marks,
            mcq.CorrectChoiceId,
            [.. mcq.Choices.Select(c => new CreateChoiceArgs(c.Id, c.Text, c.DisplayOrder))]),
        CreateEssayCommand essay => new CreateEssayArgs(essay.QuestionText, essay.Marks, essay.AnswerReference),
        _ => throw new ArgumentOutOfRangeException(nameof(command),
            $"Unsupported question command type: {command.GetType().Name}"),
    };
}
