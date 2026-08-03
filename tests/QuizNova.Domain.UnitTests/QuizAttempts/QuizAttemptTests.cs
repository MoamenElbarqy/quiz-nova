using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Enums;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Tests.Common.QuizAttempts;
using QuizNova.Tests.Common.QuizAttempts.Answers;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Quizzes.Questions;

namespace QuizNova.Domain.UnitTests.QuizAttempts;

public class QuizAttemptTests
{
    [Fact]
    public void Start_ShouldSuccess_WithValidData()
    {
        var result = QuizAttemptFactory.CreateQuizAttempt();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(QuizAttemptStatus.InProgress, result.Value.Status);
        Assert.Empty(result.Value.StudentAnswers);
    }

    [Fact]
    public void StartAttempt_OnQuiz_ShouldSuccess()
    {
        var quiz = QuizFactory.CreateQuiz(startsAtUtc: DateTimeOffset.UtcNow.AddHours(-2)).Value;
        var studentId = Guid.NewGuid();

        var result = quiz.StartAttempt(studentId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(QuizAttemptStatus.InProgress, result.Value.Status);
        Assert.Equal(quiz.Id, result.Value.QuizId);
    }

    [Fact]
    public void StartAttempt_ShouldFail_WhenStartedBeforeQuizStarts()
    {
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("StartsAtUtc")!.SetValue(quiz, DateTimeOffset.UtcNow.AddHours(1));

        var result = quiz.StartAttempt(Guid.NewGuid());

        Assert.True(result.IsError);
    }

    [Fact]
    public void StartAttempt_ShouldFail_WhenStartedAfterQuizEnds()
    {
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("EndsAtUtc")!.SetValue(quiz, DateTimeOffset.UtcNow.AddHours(-1));

        var result = quiz.StartAttempt(Guid.NewGuid());

        Assert.True(result.IsError);
    }

    [Fact]
    public void SubmitAnswer_ShouldAddAnswer_ToEmptyAttempt()
    {
        var attempt = QuizAttemptFactory.CreateQuizAttempt().Value;
        var answer = AnswerFactory.CreateTfAnswer(
            studentId: attempt.StudentId,
            questionId: Guid.NewGuid(),
            quizAttemptId: attempt.Id).Value;

        var result = attempt.SubmitAnswer(answer);

        Assert.True(result.IsSuccess);
        Assert.Single(attempt.StudentAnswers);
    }

    [Fact]
    public void SubmitAnswer_ShouldUpsert_WhenSameQuestionAnsweredAgain()
    {
        var attempt = QuizAttemptFactory.CreateQuizAttempt().Value;
        var questionId = Guid.NewGuid();
        var answer1 = AnswerFactory.CreateTfAnswer(
            studentId: attempt.StudentId, questionId: questionId,
            quizAttemptId: attempt.Id, studentChoice: true).Value;

        attempt.SubmitAnswer(answer1);

        var answer2 = AnswerFactory.CreateTfAnswer(
            studentId: attempt.StudentId, questionId: questionId,
            quizAttemptId: attempt.Id, studentChoice: false).Value;

        var result = attempt.SubmitAnswer(answer2);

        Assert.True(result.IsSuccess);
        Assert.Single(attempt.StudentAnswers);
        Assert.False(((QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer.TfAnswer)attempt.StudentAnswers.First()).StudentChoice);
    }

    [Fact]
    public void SubmitAnswer_ShouldFail_WhenAttemptNotInProgress()
    {
        var quiz = QuizFactory.CreateQuiz(startsAtUtc: DateTimeOffset.UtcNow.AddHours(-2)).Value;
        var attempt = quiz.StartAttempt(Guid.NewGuid()).Value;
        attempt.Complete(DateTime.UtcNow);

        var result = attempt.SubmitAnswer(AnswerFactory.CreateTfAnswer(
            studentId: attempt.StudentId, questionId: Guid.NewGuid(),
            quizAttemptId: attempt.Id).Value);

        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.AttemptAlreadyCompleted, result.TopError);
    }

    [Fact]
    public void Complete_ShouldSetStatus_AndSubmittedAt()
    {
        var quiz = QuizFactory.CreateQuiz(startsAtUtc: DateTimeOffset.UtcNow.AddHours(-2)).Value;
        var attempt = quiz.StartAttempt(Guid.NewGuid()).Value;
        var submittedAt = DateTime.UtcNow;

        var result = attempt.Complete(submittedAt);

        Assert.True(result.IsSuccess);
        Assert.Equal(QuizAttemptStatus.Completed, attempt.Status);
        Assert.Equal(submittedAt, attempt.SubmittedAt);
    }

    [Fact]
    public void Complete_ShouldFail_WhenAlreadyCompleted()
    {
        var quiz = QuizFactory.CreateQuiz(startsAtUtc: DateTimeOffset.UtcNow.AddHours(-2)).Value;
        var attempt = quiz.StartAttempt(Guid.NewGuid()).Value;
        attempt.Complete(DateTime.UtcNow);

        var result = attempt.Complete(DateTime.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.AttemptAlreadyCompleted, result.TopError);
    }

    [Fact]
    public void Complete_ShouldFail_WhenSubmittedAtDefault()
    {
        var attempt = QuizAttemptFactory.CreateQuizAttempt().Value;

        var result = attempt.Complete(default);

        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.SubmittedAtRequired, result.TopError);
    }

    [Fact]
    public void Complete_ShouldFail_WhenSubmittedAtBeforeStartedAt()
    {
        var quiz = QuizFactory.CreateQuiz(startsAtUtc: DateTimeOffset.UtcNow.AddHours(-2)).Value;
        var attempt = quiz.StartAttempt(Guid.NewGuid()).Value;
        var submittedAt = attempt.StartedAt.AddMinutes(-1);

        var result = attempt.Complete(submittedAt);

        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.SubmittedAtInvalid, result.TopError);
    }

    [Fact]
    public void Complete_ShouldFail_WhenSubmittedAfterQuizEnd()
    {
        var endsAtUtc = DateTimeOffset.UtcNow.AddHours(-1);
        var attempt = QuizAttemptFactory.CreateQuizAttempt(quizEndsAtUtc: endsAtUtc).Value;

        var result = attempt.Complete(DateTime.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.SubmittedAtAfterQuizEnd(endsAtUtc).Code, result.TopError.Code);
    }

    [Fact]
    public void Score_ShouldSumCorrectAnswersPolymorphically()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();

        var tfQuest = QuestionFactory.CreateTfQuestion(marks: 10).Value;
        var tfAns = AnswerFactory.CreateTfAnswer(studentId: studentId, questionId: tfQuest.Id,
            quizAttemptId: attemptId, isCorrect: true, marks: 10).Value;

        var tfQuestIncorrect = QuestionFactory.CreateTfQuestion(marks: 10).Value;
        var tfAnsIncorrect = AnswerFactory.CreateTfAnswer(studentId: studentId,
            questionId: tfQuestIncorrect.Id, quizAttemptId: attemptId, isCorrect: false, marks: 10).Value;

        var mcqQuest = QuestionFactory.CreateMcqQuestion(marks: 15).Value;
        var mcqAns = AnswerFactory.CreateMcqAnswer(studentId: studentId, quizAttemptId: attemptId,
                questionId: mcqQuest.Id, selectedChoiceId: mcqQuest.CorrectChoiceId, question: mcqQuest,
                isCorrect: true)
            .Value;

        var attempt = QuizAttemptFactory.CreateQuizAttempt(id: attemptId, studentId: studentId).Value;
        attempt.SubmitAnswer(tfAns);
        attempt.SubmitAnswer(tfAnsIncorrect);
        attempt.SubmitAnswer(mcqAns);

        Assert.Equal(25, attempt.Score);
    }

    [Fact]
    public void GradingState_ShouldBeAwaitingGrading_WhenManuallyGradedAnswersNotGraded()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var essayAns = AnswerFactory.CreateEssayAnswer(
            studentId: studentId, quizAttemptId: attemptId, score: null).Value;
        var tfAns = AnswerFactory.CreateTfAnswer(
            studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        var attempt = QuizAttemptFactory.CreateQuizAttempt(id: attemptId, studentId: studentId).Value;
        attempt.SubmitAnswer(essayAns);
        attempt.SubmitAnswer(tfAns);

        Assert.Equal(GradingState.AwaitingGrading, attempt.GradingState);
    }

    [Fact]
    public void GradingState_ShouldBeFullyGraded_WhenManuallyGradedAnswersAreGraded()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();

        var essayAns = AnswerFactory.CreateEssayAnswer(
            studentId: studentId, quizAttemptId: attemptId, score: 10).Value;
        var tfAns = AnswerFactory.CreateTfAnswer(
            studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        var attempt = QuizAttemptFactory.CreateQuizAttempt(id: attemptId, studentId: studentId).Value;
        attempt.SubmitAnswer(essayAns);
        attempt.SubmitAnswer(tfAns);

        Assert.Equal(GradingState.FullyGraded, attempt.GradingState);
    }

    [Fact]
    public void GradingState_ShouldBeFullyGraded_WhenNoManuallyGradedAnswers()
    {
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var tfAns = AnswerFactory.CreateTfAnswer(
            studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        var attempt = QuizAttemptFactory.CreateQuizAttempt(id: attemptId, studentId: studentId).Value;
        attempt.SubmitAnswer(tfAns);

        Assert.Equal(GradingState.FullyGraded, attempt.GradingState);
    }
}
