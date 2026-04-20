using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptDto>>
{
    public async Task<Result<QuizAttemptDto>> Handle(SubmitQuizAttemptCommand request, CancellationToken ct)
    {
        var studentExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Id == request.StudentId, ct);

        if (!studentExists)
        {
            return ApplicationErrors.QuizAttemptStudentNotFound(request.StudentId);
        }

        var quiz = await dbContext.Quizzes
            .Include(quizEntity => quizEntity.Questions)
            .FirstOrDefaultAsync(quizEntity => quizEntity.Id == request.QuizId, ct);

        if (quiz is null)
        {
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
            return ApplicationErrors.QuizAttemptAlreadyExists(request.StudentId, request.QuizId);
        }

        var questionsById = quiz.Questions.ToDictionary(question => question.Id);
        var submissionAnswers = new List<QuestionAnswer>(request.QuestionAnswers.Count);

        foreach (var answer in request.QuestionAnswers)
        {
            if (!questionsById.TryGetValue(answer.QuestionId, out var question))
            {
                return QuizAttemptErrors.QuestionNotFoundInQuiz(answer.QuestionId, request.QuizId);
            }

            Result<QuestionAnswer> createAnswerResult = answer switch
            {
                SubmitMcqAnswerCommand mcqAnswer => CreateMcqAnswer(
                    request,
                    question,
                    mcqAnswer),
                SubmitTrueFalseQuestionAnswerCommand trueFalseAnswer => CreateTrueFalseAnswer(
                    request,
                    question,
                    trueFalseAnswer),
                _ => Error.Unexpected("QuizAttempt.Answer.Unsupported", "Unknown answer type."),
            };

            if (createAnswerResult.IsError)
            {
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

        return createdAttempt is null
            ? Error.Unexpected("QuizAttempt.Creation.Unexpected", "Quiz attempt was created but could not be loaded.")
            : createdAttempt.ToQuizAttemptDto();
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
            Guid.NewGuid(),
            request.StudentId,
            answer.QuestionId,
            request.QuizAttemptId,
            answer.SelectedChoiceId,
            mcqQuestion);

        return createAnswerResult.IsError ? createAnswerResult.TopError : createAnswerResult.Value;
    }

    private static Result<QuestionAnswer> CreateTrueFalseAnswer(
        SubmitQuizAttemptCommand request,
        Question question,
        SubmitTrueFalseQuestionAnswerCommand answer)
    {
        if (question is not TrueFalseQuestion)
        {
            return QuizAttemptErrors.QuestionTypeMismatch(answer.QuestionId, "true-false");
        }

        var createAnswerResult = TrueFalseQuestionAnswer.Create(
            Guid.NewGuid(),
            request.StudentId,
            answer.QuestionId,
            request.QuizAttemptId,
            answer.StudentChoice);

        return createAnswerResult.IsError ? createAnswerResult.TopError : createAnswerResult.Value;
    }
}
