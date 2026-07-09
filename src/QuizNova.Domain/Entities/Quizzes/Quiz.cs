using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.QuizAttempts;

using QuizNova.Domain.Entities.Quizzes.Enums;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Domain.Entities.Quizzes;

public class Quiz : Entity
{
    private readonly List<Question> _questions;
    private readonly List<QuizAttempt> _quizAttempts;

    [SetsRequiredMembers]
    private Quiz()
    {
    }

    [SetsRequiredMembers]
    private Quiz(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        List<Question> questions)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        Title = title;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        _questions = questions;
        _quizAttempts = [];
    }

    public Guid CourseId { get; private set; }

    public Guid InstructorId { get; private set; }

    public required string Title { get; set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public int Marks => Questions.Sum(q => q.Marks);

    public IEnumerable<Question> Questions => _questions.AsReadOnly();

    public IEnumerable<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    public Course? Course { get; init; }

    public Instructor? Instructor { get; init; }

    public QuizStatus Status => DateTimeOffset.UtcNow < StartsAtUtc
        ? QuizStatus.Scheduled
        : DateTimeOffset.UtcNow >= StartsAtUtc && DateTimeOffset.UtcNow <= EndsAtUtc
            ? QuizStatus.AvailableNow
            : QuizStatus.Completed;

    public static Result<Quiz> Create(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        List<Question> questions)
    {
        if (courseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (instructorId == Guid.Empty)
        {
            return QuizErrors.InstructorIdRequired;
        }

        var trimmedTitle = title.Trim();

        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            return QuizErrors.TitleRequired;
        }

        if (trimmedTitle.Length < 3)
        {
            return QuizErrors.TitleTooShort;
        }

        if (trimmedTitle.Length > 30)
        {
            return QuizErrors.TitleTooLong;
        }

        if (startsAtUtc >= endsAtUtc)
        {
            return QuizErrors.ScheduleInvalid;
        }

        if (endsAtUtc < startsAtUtc.AddMinutes(10))
        {
            return QuizErrors.ScheduleDurationTooShort;
        }

        if (questions.Count < 1)
        {
            return QuizErrors.QuestionsRequired;
        }

        if (questions.Sum(q => q.Marks) <= 0)
        {
            return QuizErrors.MarksInvalid;
        }

        var displayOrders = questions.Select(q => q.DisplayOrder).ToHashSet();

        for (int i = 0; i < questions.Count; i++)
        {
            if (!displayOrders.Contains(i))
            {
                return QuizErrors.QuestionSequenceInvalid;
            }
        }

        if (questions.Any(q => q.QuizId != id))
        {
            var invalidQuestion = questions.First(q => q.QuizId != id);

            return QuizErrors.QuestionBelongsToDifferentQuiz(invalidQuestion.Id);
        }

        var quiz = new Quiz(
            id,
            courseId,
            instructorId,
            trimmedTitle,
            startsAtUtc,
            endsAtUtc,
            questions);

        return quiz;
    }

    public Result<Updated> Update(
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (Status != QuizStatus.Scheduled)
        {
            return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
        }

        var trimmedTitle = title.Trim();

        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            return QuizErrors.TitleRequired;
        }

        if (trimmedTitle.Length < 3)
        {
            return QuizErrors.TitleTooShort;
        }

        if (trimmedTitle.Length > 30)
        {
            return QuizErrors.TitleTooLong;
        }

        if (startsAtUtc >= endsAtUtc)
        {
            return QuizErrors.ScheduleInvalid;
        }

        if (endsAtUtc < startsAtUtc.AddMinutes(10))
        {
            return QuizErrors.ScheduleDurationTooShort;
        }

        Title = trimmedTitle;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;

        return Result.Updated;
    }

    public Result<Updated> UpdateCourseId(Guid newCourseId)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (newCourseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (Status != QuizStatus.Scheduled)
        {
            return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
        }

        CourseId = newCourseId;
        _questions.Clear();

        return Result.Updated;
    }

    public Result<Added> AddQuestion(Question question)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (Status != QuizStatus.Scheduled)
        {
            return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
        }

        if (question.QuizId != Id)
        {
            return QuizErrors.QuestionBelongsToDifferentQuiz(question.Id);
        }

        if (_questions.Any(q => q.Id == question.Id))
        {
            return QuizErrors.QuestionAlreadyExists(question.Id);
        }

        _questions.Add(question);

        return Result.Added;
    }

    public Result<Deleted> DeleteQuestion(Question question)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (Status != QuizStatus.Scheduled)
        {
            return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
        }

        if (question.QuizId != Id)
        {
            return QuizErrors.QuestionBelongsToDifferentQuiz(question.Id);
        }

        if (!_questions.Contains(question))
        {
            return QuizErrors.QuestionNotFound;
        }

        if (_questions.Count <= 1)
        {
            return QuizErrors.MinimumQuestionsReached;
        }

        _questions.Remove(question);

        return Result.Deleted;
    }

    public Result<Updated> UpdateQuestion(
        Guid questionId,
        string questionText,
        int displayOrder,
        int marks,
        Guid? correctChoiceId = null,
        bool? tfCorrectChoice = null,
        List<Choice>? choices = null,
        string? answerReference = null)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (Status != QuizStatus.Scheduled)
        {
            return QuizErrors.CannotUpdateStartedOrCompletedQuiz;
        }

        var question = _questions.FirstOrDefault(q => q.Id == questionId);

        if (question is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        return question switch
        {
            Mcq mcq when correctChoiceId.HasValue && choices is not null =>
                mcq.Update(questionText, displayOrder, marks, correctChoiceId.Value, choices),
            Tf tf when tfCorrectChoice.HasValue =>
                tf.Update(questionText, displayOrder, marks, tfCorrectChoice.Value),
            Essay essay =>
                essay.Update(questionText, displayOrder, marks, answerReference),
            _ => Error.Validation(
                "Quiz.Question.UpdateTypeMismatch",
                "The update data does not match the question type."),
        };
    }

    public Result<QuizAttempt> StartAttempt(Guid studentId)
    {
        if (Course?.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
        }

        if (studentId == Guid.Empty)
        {
            return QuizAttemptErrors.StudentIdRequired;
        }

        var startedAt = DateTimeOffset.UtcNow;

        if (startedAt < StartsAtUtc)
        {
            return QuizAttemptErrors.StartedAtBeforeQuizStart(StartsAtUtc);
        }

        if (startedAt > EndsAtUtc)
        {
            return QuizAttemptErrors.StartedAtAfterQuizEnd(EndsAtUtc);
        }

        return QuizAttempt.Start(
            Guid.NewGuid(),
            studentId,
            Id,
            startedAt.UtcDateTime);
    }

}
