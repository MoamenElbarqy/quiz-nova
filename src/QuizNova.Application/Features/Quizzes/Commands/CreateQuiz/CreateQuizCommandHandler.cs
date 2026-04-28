using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler(
    IAppDbContext dbContext,
    ILogger<CreateQuizCommandHandler> logger)
    : IRequestHandler<CreateQuizCommand, Result<QuizDetailsDto>>
{
    public async Task<Result<QuizDetailsDto>> Handle(CreateQuizCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating quiz with title: {Title} for course: {CourseId}", request.Title, request.CourseId);

        // Validate that all IDs in the request are unique and do not already exist in the database
        var allIdsInRequest = new List<Guid> { request.Id };
        allIdsInRequest.AddRange(request.Questions.Select(q => q.Id));

        var allChoiceIds = request.Questions
            .OfType<CreateMcqCommand>()
            .SelectMany(q => q.Choices.Select(c => c.Id))
            .ToList();

        allIdsInRequest.AddRange(allChoiceIds);

        if (allIdsInRequest.Distinct().Count() != allIdsInRequest.Count)
        {
            logger.LogWarning("Quiz creation failed: Duplicate IDs in request for quiz: {QuizId}", request.Id);
            return ApplicationErrors.QuizDuplicateQuestionIdsInRequest();
        }

        var existingQuizIds = await dbContext.Quizzes
            .Where(e => allIdsInRequest.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        var existingQuestionIds = await dbContext.Questions
            .Where(e => allIdsInRequest.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        var existingChoiceIds = await dbContext.Choices
            .Where(e => allIdsInRequest.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        var existingIds = existingQuizIds
            .Concat(existingQuestionIds)
            .Concat(existingChoiceIds)
            .ToList();

        if (existingIds.Any())
        {
            logger.LogWarning("Quiz creation failed: Some IDs already exist in database. First existing ID: {ExistingId}", existingIds.First());
            return ApplicationErrors.QuizSomeIdAlreadyExists(existingIds.First());
        }

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
                request.Id);

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
            request.Id,
            request.CourseId,
            request.InstructorId,
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc,
            questions.Sum(question => question.Marks),
            questions);

        if (createQuizResult.IsError)
        {
            logger.LogWarning("Quiz creation failed: Error creating quiz entity. Error: {ErrorDescription}", createQuizResult.TopError.Description);
            return createQuizResult.TopError;
        }

        await dbContext.Quizzes.AddAsync(createQuizResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Successfully created quiz {QuizId} with {QuestionCount} questions", request.Id, questions.Count);

        return createQuizResult.Value.ToQuizDetailsDto();
    }

    private Result<Question> CreateQuestion(
        CreateQuestionCommand questionCommand,
        int displayOrder,
        Guid quizId)
    {
        if (questionCommand.QuizId != quizId)
        {
            return QuizErrors.QuestionBelongsToDifferentQuiz(questionCommand.Id);
        }

        return questionCommand switch
        {
            CreateTfCommand tfQuestion =>
                CreateTf(tfQuestion, displayOrder),
            CreateMcqCommand mcq => CreateMcq(mcq, displayOrder),
            _ => Error.Unexpected(
                "Quiz.Question.Unsupported",
                $"Unsupported question type '{questionCommand.GetType().Name}'."),
        };
    }

    private Result<Question> CreateTf(CreateTfCommand command, int displayOrder)
    {
        var result = Tf.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            command.CorrectChoice,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private Result<Question> CreateMcq(
        CreateMcqCommand command,
        int displayOrder)
    {
        if (command.Choices.All(choice => choice.Id != command.CorrectChoiceId))
        {
            return ApplicationErrors.QuizCorrectChoiceNotFound(command.Id, command.CorrectChoiceId);
        }

        if (command.Choices.GroupBy(choice => choice.Id).Any(group => group.Count() > 1))
        {
            return ApplicationErrors.QuizChoiceIdsMustBeUnique(command.Id);
        }

        var choices = new List<Choice>(command.Choices.Count);

        foreach (var choiceCommand in command.Choices)
        {
            if (choiceCommand.QuestionId != command.Id)
            {
                return ApplicationErrors.QuizChoiceBelongsToDifferentQuestion(choiceCommand.Id, command.Id);
            }

            var createChoiceResult = Choice.Create(
                choiceCommand.Id,
                choiceCommand.QuestionId,
                choiceCommand.Text,
                choiceCommand.DisplayOrder);

            if (createChoiceResult.IsError)
            {
                return createChoiceResult.TopError;
            }

            choices.Add(createChoiceResult.Value);
        }

        var createQuestionResult = Mcq.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            command.NumberOfChoices,
            command.CorrectChoiceId,
            displayOrder,
            command.Marks,
            choices);

        return createQuestionResult.IsError ? createQuestionResult.TopError : createQuestionResult.Value;
    }
}
