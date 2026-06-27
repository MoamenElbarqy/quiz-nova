using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler(
    IAppDbContext dbContext,
    ILogger<CreateQuizCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(CreateQuizCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Creating quiz with title: {Title} for course: {CourseId}",
            request.Title,
            request.CourseId);

        var quizId = Guid.NewGuid();

        if (!await dbContext.Courses.AnyAsync(course => course.Id == request.CourseId, ct))
        {
            logger.LogWarning("Quiz creation failed: Course {CourseId} not found", request.CourseId);
            return ApplicationErrors.QuizCourseNotFound(request.CourseId);
        }

        if (!await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct))
        {
            logger.LogWarning("Quiz creation failed: Instructor {InstructorId} not found", request.InstructorId);
            return ApplicationErrors.QuizInstructorNotFound(request.InstructorId);
        }

        if (!await dbContext.Courses.AnyAsync(
                course => course.Id == request.CourseId && course.InstructorId == request.InstructorId,
                ct))
        {
            logger.LogWarning(
                "Quiz creation failed: Instructor {InstructorId} is not assigned to course {CourseId}",
                request.InstructorId,
                request.CourseId);

            return ApplicationErrors.QuizInstructorIsNotAssignedToCourse(request.InstructorId, request.CourseId);
        }

        var questions = new List<Question>(request.Questions.Count);

        foreach (var indexedQuestion in request.Questions.Select((question, index) => new { question, index }))
        {
            var createQuestionResult = CreateQuestion(
                indexedQuestion.question,
                indexedQuestion.index,
                quizId);

            if (createQuestionResult.IsError)
            {
                logger.LogWarning(
                    "Quiz creation failed: Error creating question at index {Index}. Error: {ErrorDescription}",
                    indexedQuestion.index,
                    createQuestionResult.TopError.Description);

                return createQuestionResult.TopError;
            }

            questions.Add(createQuestionResult.Value);
        }

        var createQuizResult = Quiz.Create(
            quizId,
            request.CourseId,
            request.InstructorId,
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc,
            questions);

        if (createQuizResult.IsError)
        {
            logger.LogWarning(
                "Quiz creation failed: Error creating quiz entity. Error: {ErrorDescription}",
                createQuizResult.TopError.Description);
            return createQuizResult.TopError;
        }

        await dbContext.Quizzes.AddAsync(createQuizResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);
        await cacheInvalidator.InvalidateAsync(["quizzes"], ct);

        logger.LogInformation("Successfully created quiz {QuizId} with {QuestionCount} questions", quizId,
            questions.Count);

        return createQuizResult.Value.ToQuizDto();
    }

    private Result<Question> CreateQuestion(
        CreateQuestionCommand questionCommand,
        int displayOrder,
        Guid quizId)
    {
        return questionCommand switch
        {
            CreateTfCommand tfQuestion =>
                CreateTf(tfQuestion, displayOrder, quizId),
            CreateMcqCommand mcq => CreateMcq(mcq, displayOrder, quizId),
            CreateEssayCommand essay => CreateEssay(essay, displayOrder, quizId),
            _ => Error.Unexpected(
                "Quiz.Question.Unsupported",
                $"Unsupported question type '{questionCommand.GetType().Name}'."),
        };
    }

    private Result<Question> CreateTf(CreateTfCommand command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();
        var result = Tf.Create(
            questionId,
            quizId,
            command.QuestionText,
            command.CorrectChoice,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private Result<Question> CreateMcq(
        CreateMcqCommand command,
        int displayOrder,
        Guid quizId)
    {
        var questionId = Guid.NewGuid();

        if (command.Choices.All(choice => choice.Id != command.CorrectChoiceId))
        {
            return ApplicationErrors.QuizCorrectChoiceNotFound(questionId, command.CorrectChoiceId);
        }

        var choices = new List<Choice>(command.Choices.Count);
        var actualCorrectChoiceId = Guid.Empty;

        foreach (var choiceCommand in command.Choices)
        {
            var choiceId = Guid.NewGuid();
            if (choiceCommand.Id == command.CorrectChoiceId)
            {
                actualCorrectChoiceId = choiceId;
            }

            var createChoiceResult = Choice.Create(
                choiceId,
                questionId,
                choiceCommand.Text,
                choiceCommand.DisplayOrder);

            if (createChoiceResult.IsError)
            {
                return createChoiceResult.TopError;
            }

            choices.Add(createChoiceResult.Value);
        }

        var createQuestionResult = Mcq.Create(
            questionId,
            quizId,
            command.QuestionText,
            actualCorrectChoiceId,
            displayOrder,
            command.Marks,
            choices);

        return createQuestionResult.IsError ? createQuestionResult.TopError : createQuestionResult.Value;
    }

    private Result<Question> CreateEssay(CreateEssayCommand command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();
        var result = Essay.Create(
            questionId,
            quizId,
            command.QuestionText,
            command.AnswerReference,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }
}
