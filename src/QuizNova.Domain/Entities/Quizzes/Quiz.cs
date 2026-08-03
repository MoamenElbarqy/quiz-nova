using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes.Enums;
using QuizNova.Domain.Entities.Quizzes.Questions;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Domain.Entities.Quizzes;

public class Quiz : Entity
{
    private List<Question> _questions = [];

    [SetsRequiredMembers]
    private Quiz()
    {
    }

    [SetsRequiredMembers]
    private Quiz(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string courseName,
        string instructorName,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        int marks,
        List<Question> questions)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        CourseName = courseName;
        InstructorName = instructorName;
        Title = title;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Marks = marks;
        _questions = questions;
    }

    public Guid CourseId { get; private set; }

    public Guid InstructorId { get; private set; }

    public required string CourseName { get; set; }

    public required string InstructorName { get; set; }

    public required string Title { get; set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public int Marks { get; private set; }

    public IEnumerable<Question> Questions
    {
        get => _questions.AsReadOnly();
        private set => _questions = [.. value];
    }

    public QuizStatus Status => DateTimeOffset.UtcNow < StartsAtUtc
        ? QuizStatus.Scheduled
        : DateTimeOffset.UtcNow >= StartsAtUtc && DateTimeOffset.UtcNow <= EndsAtUtc
            ? QuizStatus.AvailableNow
            : QuizStatus.Completed;

    public static Result<Quiz> Create(
        Guid id,
        Guid courseId,
        Guid instructorId,
        string courseName,
        string instructorName,
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        IEnumerable<CreateQuestionArgs> questionArgs,
        Course course)
    {
        if (courseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (course.Id != courseId)
        {
            return QuizErrors.CourseMismatch(course.Id, courseId);
        }

        if (course.Status == CourseStatus.Completed)
        {
            return QuizErrors.CourseCompleted;
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

        var questionsList = questionArgs.ToList();

        if (questionsList.Count < 1)
        {
            return QuizErrors.QuestionsRequired;
        }

        var questions = new List<Question>(questionsList.Count);

        for (int index = 0; index < questionsList.Count; index++)
        {
            var questionArg = questionsList[index];
            var createQuestionResult = Question.CreateFromArgs(questionArg, index, id);

            if (createQuestionResult.IsError)
            {
                return createQuestionResult.TopError;
            }

            questions.Add(createQuestionResult.Value);
        }

        var totalMarks = questions.Sum(q => q.Marks);

        if (totalMarks <= 0)
        {
            return QuizErrors.MarksInvalid;
        }

        var consumeResult = course.ConsumeMarks(totalMarks);
        if (consumeResult.IsError)
        {
            return consumeResult.TopError;
        }

        var quiz = new Quiz(
            id,
            courseId,
            instructorId,
            courseName,
            instructorName,
            trimmedTitle,
            startsAtUtc,
            endsAtUtc,
            totalMarks,
            questions);

        return quiz;
    }

    public Result<Updated> Update(
        string title,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc)
    {
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

    public Result<Added> AddQuestion(Question question, Course course)
    {
        if (course.Id != CourseId)
        {
            return QuizErrors.CourseMismatch(course.Id, CourseId);
        }

        if (course.Status == CourseStatus.Completed)
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

        var consumeResult = course.ConsumeMarks(question.Marks);
        if (consumeResult.IsError)
        {
            return consumeResult.TopError;
        }

        _questions.Add(question);

        return Result.Added;
    }

    public Result<Deleted> DeleteQuestion(Question question, Course course)
    {
        if (course.Id != CourseId)
        {
            return QuizErrors.CourseMismatch(course.Id, CourseId);
        }

        if (course.Status == CourseStatus.Completed)
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

        var releaseResult = course.ReleaseMarks(question.Marks);
        if (releaseResult.IsError)
        {
            return releaseResult.TopError;
        }

        _questions.Remove(question);

        return Result.Deleted;
    }

    public Result<Updated> UpdateQuestion(
        Guid questionId,
        string questionText,
        int displayOrder,
        int marks,
        Course course,
        Guid? correctChoiceId,
        bool? tfCorrectChoice,
        List<Choice>? choices,
        string? answerReference)
    {
        if (course.Id != CourseId)
        {
            return QuizErrors.CourseMismatch(course.Id, CourseId);
        }

        if (course.Status == CourseStatus.Completed)
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

        var marksDelta = marks - question.Marks;
        if (marksDelta > 0)
        {
            var consumeResult = course.ConsumeMarks(marksDelta);
            if (consumeResult.IsError)
            {
                return consumeResult.TopError;
            }
        }
        else if (marksDelta < 0)
        {
            var releaseResult = course.ReleaseMarks(-marksDelta);
            if (releaseResult.IsError)
            {
                return releaseResult.TopError;
            }
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
            startedAt.UtcDateTime,
            EndsAtUtc);
    }
}
