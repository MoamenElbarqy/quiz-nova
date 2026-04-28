using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler(
    IAppDbContext dbContext,
    ILogger<SubmitQuizAttemptCommandHandler> logger)
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(SubmitQuizAttemptCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Submitting quiz attempt {QuizAttemptId} for student {StudentId} and quiz {QuizId}",
            request.QuizAttemptId,
            request.StudentId,
            request.QuizId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Quiz attempt submission failed: Student {StudentId} not found", request.StudentId);
            return ApplicationErrors.QuizAttemptStudentNotFound(request.StudentId);
        }

        var quiz = await dbContext.Quizzes
            .Include(quizEntity => quizEntity.Questions)
            .FirstOrDefaultAsync(quizEntity => quizEntity.Id == request.QuizId, ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz attempt submission failed: Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        await dbContext.Questions
            .OfType<Mcq>()
            .Where(question => question.QuizId == request.QuizId)
            .Include(question => question.Choices)
            .LoadAsync(ct);

        var isStudentEnrolledInCourse = await dbContext.StudentCourses
            .AsNoTracking()
            .AnyAsync(
                studentCourse => studentCourse.StudentId == request.StudentId &&
                                 studentCourse.CourseId == quiz.CourseId,
                ct);

        if (!isStudentEnrolledInCourse)
        {
            logger.LogWarning(
                "Quiz attempt submission failed: Student {StudentId} is not enrolled in course {CourseId}",
                request.StudentId,
                quiz.CourseId);

            return ApplicationErrors.StudentNotEnrolledInCourse(request.StudentId, quiz.CourseId);
        }

        var attemptAlreadyExists = await dbContext.QuizAttempts
            .AsNoTracking()
            .AnyAsync(
                quizAttempt => quizAttempt.StudentId == request.StudentId &&
                               quizAttempt.QuizId == request.QuizId,
                ct);

        if (attemptAlreadyExists)
        {
            logger.LogWarning(
                "Quiz attempt submission failed: Attempt already exists for student {StudentId} and quiz {QuizId}",
                request.StudentId,
                request.QuizId);

            return ApplicationErrors.QuizAttemptAlreadyExists(request.StudentId, request.QuizId);
        }

        var questionsById = quiz.Questions.ToDictionary(question => question.Id);
        var submissionAnswers = new List<QuestionAnswer>(request.QuestionAnswers.Count);

        foreach (var answer in request.QuestionAnswers)
        {
            if (!questionsById.TryGetValue(answer.QuestionId, out var question))
            {
                logger.LogWarning(
                    "Quiz attempt submission failed: Question {QuestionId} not found in quiz {QuizId}",
                    answer.QuestionId,
                    request.QuizId);

                return QuizAttemptErrors.QuestionNotFoundInQuiz(answer.QuestionId, request.QuizId);
            }

            Result<QuestionAnswer> createAnswerResult = answer switch
            {
                SubmitMcqAnswerCommand mcqAnswer => CreateMcqAnswer(
                    request,
                    question,
                    mcqAnswer),
                SubmitTfAnswerCommand tfAnswer => CreateTfAnswer(
                    request,
                    question,
                    tfAnswer),
                _ => Error.Unexpected("QuizAttempt.Answer.Unsupported", "Unknown answer type."),
            };

            if (createAnswerResult.IsError)
            {
                logger.LogWarning(
                    "Quiz attempt submission failed: Error creating answer for question {QuestionId}. Error: {ErrorDescription}",
                    answer.QuestionId,
                    createAnswerResult.TopError.Description);

                return createAnswerResult.TopError;
            }

            submissionAnswers.Add(createAnswerResult.Value);
        }

        var createAttemptResult = quiz.SubmitAttempt(
            request.QuizAttemptId,
            request.StudentId,
            request.QuizId,
            request.StartedAt,
            request.SubmittedAt,
            submissionAnswers);

        if (createAttemptResult.IsError)
        {
            logger.LogWarning(
                "Quiz attempt submission failed: Domain error during attempt submission. Error: {ErrorDescription}",
                createAttemptResult.TopError.Description);

            return createAttemptResult.TopError;
        }

        await dbContext.QuizAttempts.AddAsync(createAttemptResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        var createdAttempt = await dbContext.QuizAttempts
            .AsNoTracking()
            .Where(quizAttempt => quizAttempt.Id == request.QuizAttemptId)
            .Include(quizAttempt => quizAttempt.Quiz)
            .ThenInclude(quizEntity => quizEntity!.Questions)
            .Include(quizAttempt => quizAttempt.StudentAnswers)
            .FirstOrDefaultAsync(ct);

        if (createdAttempt is null)
        {
            logger.LogError(
                "Quiz attempt submission failed: Created attempt {QuizAttemptId} could not be re-loaded from database",
                request.QuizAttemptId);

            return Error.Unexpected("QuizAttempt.Creation.Unexpected", "Quiz attempt was created but could not be loaded.");
        }

        logger.LogInformation(
            "Successfully submitted quiz attempt {QuizAttemptId} for student {StudentId}. Score: {Score}",
            request.QuizAttemptId,
            request.StudentId,
            createdAttempt.Score);

        return createdAttempt.ToQuizAttemptDto();
    }

    private static Result<QuestionAnswer> CreateMcqAnswer(
        SubmitQuizAttemptCommand request,
        Question question,
        SubmitMcqAnswerCommand answer)
    {
        if (question is not Mcq mcqQuestion)
        {
            return QuizAttemptErrors.QuestionTypeMismatch(answer.QuestionId, "mcq");
        }

        var createAnswerResult = McqAnswer.Create(
            answer.AnswerId,
            request.StudentId,
            answer.QuestionId,
            request.QuizAttemptId,
            answer.SelectedChoiceId,
            mcqQuestion);

        return createAnswerResult.IsError ? createAnswerResult.TopError : createAnswerResult.Value;
    }

    private static Result<QuestionAnswer> CreateTfAnswer(
        SubmitQuizAttemptCommand request,
        Question question,
        SubmitTfAnswerCommand answer)
    {
        if (question is not Tf)
        {
            return QuizAttemptErrors.QuestionTypeMismatch(answer.QuestionId, "tf");
        }

        var createAnswerResult = TfAnswer.Create(
            Guid.NewGuid(),
            request.StudentId,
            answer.QuestionId,
            request.QuizAttemptId,
            answer.StudentChoice);

        return createAnswerResult.IsError ? createAnswerResult.TopError : createAnswerResult.Value;
    }
}
