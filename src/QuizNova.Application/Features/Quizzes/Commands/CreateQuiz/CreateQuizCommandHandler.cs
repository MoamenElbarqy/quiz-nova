using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Essay;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler
    : IRequestHandler<CreateQuizCommand, Result<QuizDetailsDto>>
{
    private readonly IAppDbContext dbContext;

    public CreateQuizCommandHandler(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Result<QuizDetailsDto>> Handle(CreateQuizCommand request, CancellationToken ct)
    {
        // Validate that all IDs in the request are unique and do not already exist in the database
        var allIdsInRequest = new List<Guid> { request.Id };
        allIdsInRequest.AddRange(request.Questions.Select(q => q.Id));

        var allChoiceIds = request.Questions
            .OfType<CreateMultipleChoiceQuestionCommand>()
            .SelectMany(q => q.Choices.Select(c => c.Id))
            .ToList();

        allIdsInRequest.AddRange(allChoiceIds);

        if (allIdsInRequest.Distinct().Count() != allIdsInRequest.Count)
        {
            return ApplicationErrors.QuizDuplicateQuestionIdsInRequest();
        }

        var existingIds = await dbContext.Entities
            .Where(e => allIdsInRequest.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        if (existingIds.Any())
        {
            return ApplicationErrors.QuizSomeIdAlreadyExists(existingIds.First());
        }

        if (!await dbContext.Courses.AnyAsync(course => course.Id == request.CourseId, ct))
        {
            return ApplicationErrors.QuizCourseNotFound(request.CourseId);
        }

        if (!await dbContext.Instructors.AnyAsync(instructor => instructor.Id == request.InstructorId, ct))
        {
            return ApplicationErrors.QuizInstructorNotFound(request.InstructorId);
        }

        if (!await dbContext.Courses.AnyAsync(
                course => course.Id == request.CourseId && course.InstructorId == request.InstructorId,
                ct))
        {
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
            return createQuizResult.TopError;
        }

        await dbContext.Quizzes.AddAsync(createQuizResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

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
            CreateEssayQuestionCommand essayQuestion => CreateEssayQuestion(essayQuestion, displayOrder),
            CreateTrueFalseQuestionCommand trueFalseQuestion => CreateTrueFalseQuestion(trueFalseQuestion, displayOrder),
            CreateMultipleChoiceQuestionCommand multipleChoiceQuestion => CreateMultipleChoiceQuestion(multipleChoiceQuestion, displayOrder),
            _ => Error.Unexpected("Quiz.Question.Unsupported", $"Unsupported question type '{questionCommand.GetType().Name}'."),
        };
    }

    private Result<Question> CreateEssayQuestion(CreateEssayQuestionCommand command, int displayOrder)
    {
        var result = EssayQuestion.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private Result<Question> CreateTrueFalseQuestion(CreateTrueFalseQuestionCommand command, int displayOrder)
    {
        var result = TrueFalseQuestion.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            command.CorrectChoice,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private Result<Question> CreateMultipleChoiceQuestion(
        CreateMultipleChoiceQuestionCommand command,
        int displayOrder)
    {
        if (!command.Choices.Any(choice => choice.Id == command.CorrectChoiceId))
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

        var createQuestionResult = McqQuestion.Create(
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
