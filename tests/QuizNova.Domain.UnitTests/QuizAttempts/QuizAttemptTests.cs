using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Enums;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.QuizAttempts;
using QuizNova.Tests.Common.QuizAttempts.Answers;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Quizzes.Questions;

namespace QuizNova.Domain.UnitTests.QuizAttempts;

public class QuizAttemptTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Act
        var result = QuizAttemptFactory.CreateQuizAttempt();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void SubmitAttempt_ShouldSuccess_WithExactAnswersCount()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var questions = quiz.Questions.ToList();
        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[0].Id, quizAttemptId: attemptId).Value;
        var ans2 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[1].Id, quizAttemptId: attemptId).Value;
        var ans3 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[2].Id, quizAttemptId: attemptId).Value;

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1, ans2, ans3]);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.StudentAnswers.Count());
    }

    [Fact]
    public void SubmitAttempt_ShouldSuccess_WithFewerAnswersThanQuestions()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var questions = quiz.Questions.ToList();

        // Submitting only 2 answers for a 3-question quiz
        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[0].Id, quizAttemptId: attemptId).Value;
        var ans2 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[1].Id, quizAttemptId: attemptId).Value;

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1, ans2]);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.StudentAnswers.Count());
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WithMoreAnswersThanQuestions()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var questions = quiz.Questions.ToList();
        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[0].Id, quizAttemptId: attemptId).Value;
        var ans2 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[1].Id, quizAttemptId: attemptId).Value;
        var ans3 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[2].Id, quizAttemptId: attemptId).Value;
        var ans4 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1, ans2, ans3, ans4]);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.TooManyQuestionAnswers(4, 3).Code, result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenQuizIdMismatch()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;

        // Act
        var result = quiz.SubmitAttempt(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow, []);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenStartsAtEqualOrAfterSubmittedAt()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(20),
            quiz.StartsAtUtc.AddMinutes(10), // Submitted before started!
            []);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.SubmittedAtInvalid, result.TopError);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenSubmittedAfterQuizEnd()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.EndsAtUtc.AddMinutes(1), // Submitted after end time!
            []);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.SubmittedAtAfterQuizEnd(quiz.EndsAtUtc).Code, result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenStartedBeforeQuizStarts()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(-1), // Started before quiz start time!
            quiz.StartsAtUtc.AddMinutes(5),
            []);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.StartedAtBeforeQuizStart(quiz.StartsAtUtc).Code, result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenSubmissionDoesNotRelateToQuiz()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var questions = quiz.Questions.ToList();

        // Answer belongs to a completely different attempt
        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: questions[0].Id, quizAttemptId: Guid.NewGuid()).Value;

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1]);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.AnswerQuizAttemptMismatch(ans1.QuestionId, attemptId, ans1.QuizAttemptId).Code,
            result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenSubmissionDoesNotRelateToStudent()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var questions = quiz.Questions.ToList();

        // Answer belongs to a different student
        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: Guid.NewGuid(), questionId: questions[0].Id, quizAttemptId: attemptId).Value;

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1]);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.AnswerStudentMismatch(ans1.QuestionId, studentId, ans1.StudentId).Code,
            result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenSubmissionDoesNotRelateToAnyQuestionInQuiz()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var attemptId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var ans1 = AnswerFactory
            .CreateTfAnswer(studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId)
            .Value; // Guid.NewGuid is not in quiz!

        // Act
        var result = quiz.SubmitAttempt(
            attemptId,
            studentId,
            quiz.Id,
            quiz.StartsAtUtc.AddMinutes(5),
            quiz.StartsAtUtc.AddMinutes(20),
            [ans1]);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizAttemptErrors.QuestionNotFoundInQuiz(ans1.QuestionId, quiz.Id).Code, result.TopError.Code);
    }

    [Fact]
    public void SubmitAttempt_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();
        var completedQuiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(completedQuiz, course);

        // Act
        var result = completedQuiz.SubmitAttempt(Guid.NewGuid(), Guid.NewGuid(), completedQuiz.Id,
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, []);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }

    [Fact]
    public void Score_ShouldSumCorrectAnswersPolymorphically()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();

        // 1. Correct TF Answer (10 marks)
        var tfQuest = QuestionFactory.CreateTfQuestion(marks: 10).Value;
        var tfAns = AnswerFactory.CreateTfAnswer(studentId: studentId, questionId: tfQuest.Id,
            quizAttemptId: attemptId, isCorrect: true).Value;
        typeof(QuestionAnswer).GetProperty("Question")!.SetValue(tfAns, tfQuest);

        // 2. Incorrect TF Answer (marks 10, but incorrect so 0)
        var tfQuestIncorrect = QuestionFactory.CreateTfQuestion(marks: 10).Value;
        var tfAnsIncorrect = AnswerFactory.CreateTfAnswer(studentId: studentId,
            questionId: tfQuestIncorrect.Id, quizAttemptId: attemptId, isCorrect: false).Value;
        typeof(QuestionAnswer).GetProperty("Question")!.SetValue(tfAnsIncorrect, tfQuestIncorrect);

        // 3. Correct MCQ Answer (15 marks)
        var mcqQuest = QuestionFactory.CreateMcqQuestion(marks: 15).Value;
        var mcqAns = AnswerFactory.CreateMcqAnswer(studentId: studentId, quizAttemptId: attemptId,
                questionId: mcqQuest.Id, selectedChoiceId: mcqQuest.CorrectChoiceId, question: mcqQuest,
                isCorrect: true)
            .Value;
        typeof(QuestionAnswer).GetProperty("Question")!.SetValue(mcqAns, mcqQuest);

        // Act
        var attempt = QuizAttemptFactory.CreateQuizAttempt(studentAnswers: [tfAns, tfAnsIncorrect, mcqAns],
            id: attemptId, studentId: studentId).Value;

        // Assert
        Assert.Equal(25, attempt.Score); // 10 + 0 + 15 = 25
    }

    [Fact]
    public void Create_ShouldSetStatusToPending_WhenManuallyGradedAnswersExistAndNotGradedYet()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var essayAns = AnswerFactory.CreateEssayAnswer(studentId: studentId, quizAttemptId: attemptId, score: null)
            .Value;
        var tfAns = AnswerFactory.CreateTfAnswer(studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        // Act
        var attempt = QuizAttemptFactory
            .CreateQuizAttempt(studentAnswers: [essayAns, tfAns], id: attemptId, studentId: studentId).Value;

        // Assert
        Assert.Equal(QuizAttemptStatus.Pending, attempt.Status);
    }

    [Fact]
    public void Create_ShouldSetStatusToCompleted_WhenManuallyGradedAnswersExistAndAreGraded()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();

        // Give it a score so it is graded
        var essayAns = AnswerFactory.CreateEssayAnswer(studentId: studentId, quizAttemptId: attemptId, score: 10).Value;
        var tfAns = AnswerFactory.CreateTfAnswer(studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        // Act
        var attempt = QuizAttemptFactory
            .CreateQuizAttempt(studentAnswers: [essayAns, tfAns], id: attemptId, studentId: studentId).Value;

        // Assert
        Assert.Equal(QuizAttemptStatus.Completed, attempt.Status);
    }

    [Fact]
    public void Create_ShouldSetStatusToCompleted_WhenNoManuallyGradedAnswersExist()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var attemptId = Guid.NewGuid();
        var tfAns = AnswerFactory.CreateTfAnswer(studentId: studentId, questionId: Guid.NewGuid(), quizAttemptId: attemptId).Value;

        // Act
        var attempt = QuizAttemptFactory.CreateQuizAttempt(studentAnswers: [tfAns], id: attemptId, studentId: studentId)
            .Value;

        // Assert
        Assert.Equal(QuizAttemptStatus.Completed, attempt.Status);
    }
}
