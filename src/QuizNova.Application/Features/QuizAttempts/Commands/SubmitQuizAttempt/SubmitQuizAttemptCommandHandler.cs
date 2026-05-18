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
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler(
    IAppDbContext dbContext,
    IUser user,
    ILogger<SubmitQuizAttemptCommandHandler> logger)
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(SubmitQuizAttemptCommand request, CancellationToken ct)
    {
        var studentId = Guid.Parse(user.Id!);

        var quizAttemptId = Guid.NewGuid();

        logger.LogInformation(
            "Submitting quiz attempt {QuizAttemptId} for student {StudentId} and quiz {QuizId}",
            quizAttemptId,
            studentId,
            request.QuizId);

        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == studentId, ct);

        if (!studentExists)
        {
            logger.LogWarning("Quiz attempt submission failed: Student {StudentId} not found", studentId);
            return ApplicationErrors.QuizAttemptStudentNotFound(studentId);
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

        var isStudentEnrolledInCourse = await dbContext.Enrollments
            .AsNoTracking()
            .AnyAsync(
                enrollment => enrollment.StudentId == studentId &&
                                 enrollment.CourseId == quiz.CourseId,
                ct);

        if (!isStudentEnrolledInCourse)
        {
            logger.LogWarning(
                "Quiz attempt submission failed: Student {StudentId} is not enrolled in course {CourseId}",
                studentId,
                quiz.CourseId);

            return ApplicationErrors.StudentNotEnrolledInCourse(studentId, quiz.CourseId);
        }

        var attemptAlreadyExists = await dbContext.QuizAttempts
            .AsNoTracking()
            .AnyAsync(
                quizAttempt => quizAttempt.StudentId == studentId &&
                               quizAttempt.QuizId == request.QuizId,
                ct);

        if (attemptAlreadyExists)
        {
            logger.LogWarning(
                "Quiz attempt submission failed: Attempt already exists for student {StudentId} and quiz {QuizId}",
                studentId,
                request.QuizId);

            return ApplicationErrors.QuizAttemptAlreadyExists(studentId, request.QuizId);
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

            Result<QuestionAnswer> createAnswerResult = (question, answer) switch
            {
                (Mcq mcqQuestion, SubmitMcqAnswerCommand mcqAnswer) =>
                    mcqQuestion.Solve(mcqAnswer.SelectedChoiceId, studentId, quizAttemptId),
                (Tf tfQuestion, SubmitTfAnswerCommand tfAnswer) =>
                    tfQuestion.Solve(tfAnswer.StudentChoice, studentId, quizAttemptId),
                (Mcq, _) => Error.Unexpected(
                    "QuizAttempt.Answer.AnswerTypeMismatch",
                    $"Question {answer.QuestionId} is an MCQ question but the submitted answer is not an MCQ answer."),
                (Tf, _) => Error.Unexpected(
                    "QuizAttempt.Answer.AnswerTypeMismatch",
                    $"Question {answer.QuestionId} is a True/False question but the submitted answer is not a True/False answer."),
                _ => Error.Unexpected(
                    "QuizAttempt.Answer.Unsupported",
                    $"Question {answer.QuestionId} has an unsupported question type."),
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
            quizAttemptId,
            studentId,
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

        logger.LogInformation(
            "Successfully submitted quiz attempt {QuizAttemptId} for student {StudentId}. Score: {Score}",
            quizAttemptId,
            studentId,
            createAttemptResult.Value.Score);

        return createAttemptResult.Value.ToQuizAttemptDto();
    }
}
