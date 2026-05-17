using System.Diagnostics.CodeAnalysis;
using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;
using QuizNova.Domain.Entities.Quizzes.Enums;
using QuizNova.Domain.Entities.Quizzes.Events;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Domain.Entities.Quizzes;

public class Quiz : Entity
{
    private readonly List<Question> _questions;

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
    }

    public Guid CourseId { get; private set; }

    public Guid InstructorId { get; private set; }

    public required string Title { get; set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public int Marks => Questions.Sum(q => q.Marks);

    public IEnumerable<Question> Questions => _questions.AsReadOnly();

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
        int marks,
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

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (startsAtUtc >= endsAtUtc)
        {
            return QuizErrors.ScheduleInvalid;
        }

        if (questions.Sum(q => q.Marks) <= 0)
        {
            return QuizErrors.MarksInvalid;
        }

        if (questions.Count == 0)
        {
            return QuizErrors.QuestionsRequired;
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
            title,
            startsAtUtc,
            endsAtUtc,
            questions);
        quiz.AddDomainEvent(new QuizCreatedEvent(id));
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

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (startsAtUtc >= endsAtUtc)
        {
            return QuizErrors.ScheduleInvalid;
        }

        Title = title;
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

    public Result<Added> AddQuestion(Question question)
    {
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

        if (_questions.Count <= 5)
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
        Guid? correctChoiceId,
        bool? tfCorrectChoice,
        List<Choice>? choices)
    {
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
            _ => Error.Validation(
                "Quiz.Question.UpdateTypeMismatch",
                "The update data does not match the question type."),
        };
    }

    public Result<QuizAttempt> SubmitAttempt(
        Guid attemptId,
        Guid studentId,
        Guid quizId,
        DateTimeOffset startedAt,
        DateTimeOffset submittedAt,
        IReadOnlyCollection<QuestionAnswer> questionAnswers)
    {
        if (attemptId == Guid.Empty)
        {
            return QuizAttemptErrors.AttemptIdRequired;
        }

        if (quizId == Guid.Empty)
        {
            return QuizAttemptErrors.QuizIdRequired;
        }

        if (quizId != Id)
        {
            return QuizAttemptErrors.QuizIdMismatch(Id, quizId);
        }

        if (startedAt == default)
        {
            return QuizAttemptErrors.StartedAtRequired;
        }

        if (submittedAt == default)
        {
            return QuizAttemptErrors.SubmittedAtRequired;
        }

        if (startedAt >= submittedAt)
        {
            return QuizAttemptErrors.SubmittedAtInvalid;
        }

        if (submittedAt > EndsAtUtc)
        {
            return QuizAttemptErrors.SubmittedAtAfterQuizEnd(EndsAtUtc);
        }

        if (startedAt < StartsAtUtc)
        {
            return QuizAttemptErrors.StartedAtBeforeQuizStart(StartsAtUtc);
        }

        if (questionAnswers.Count == 0)
        {
            return QuizAttemptErrors.QuestionAnswersRequired;
        }

        if (questionAnswers.Count > _questions.Count)
        {
            return QuizAttemptErrors.TooManyQuestionAnswers(questionAnswers.Count, _questions.Count);
        }

        if (questionAnswers.GroupBy(answer => answer.QuestionId).Any(group => group.Count() > 1))
        {
            return QuizAttemptErrors.DuplicateQuestionAnswers;
        }

        foreach (var answer in questionAnswers)
        {
            var question = _questions.FirstOrDefault(candidateQuestion => candidateQuestion.Id == answer.QuestionId);

            if (question is null)
            {
                return QuizAttemptErrors.QuestionNotFoundInQuiz(answer.QuestionId, Id);
            }

            var answerValidation = ValidateAnswer(
                attemptId,
                studentId,
                question,
                answer);

            if (answerValidation.IsError)
            {
                return answerValidation.TopError;
            }
        }

        return QuizAttempt.Create(
            attemptId,
            studentId,
            Id,
            startedAt.UtcDateTime,
            submittedAt.UtcDateTime,
            questionAnswers.ToList());
    }

    private static Result<Validated> ValidateAnswer(
        Guid attemptId,
        Guid studentId,
        Question question,
        QuestionAnswer answer)
    {
        if (answer.QuizAttemptId != attemptId)
        {
            return QuizAttemptErrors.AnswerQuizAttemptMismatch(answer.QuestionId, attemptId, answer.QuizAttemptId);
        }

        if (answer.StudentId != studentId)
        {
            return QuizAttemptErrors.AnswerStudentMismatch(answer.QuestionId, studentId, answer.StudentId);
        }

        return answer switch
        {
            McqAnswer mcqAnswer => ValidateMcqAnswer(question, mcqAnswer),
            TfAnswer tfAnswer => ValidateTfAnswer(question, tfAnswer),
            _ => Error.Unexpected(
                "QuizAttempt.Answer.Unsupported",
                $"Unsupported answer type '{answer.GetType().Name}'."),
        };
    }

    private static Result<Validated> ValidateMcqAnswer(Question question, McqAnswer answer)
    {
        if (question is not Mcq mcqQuestion)
        {
            return QuizAttemptErrors.QuestionTypeMismatch(answer.QuestionId, "mcq");
        }

        if (mcqQuestion.Choices.All(choice => choice.Id != answer.SelectedChoiceId))
        {
            return McqAnswerErrors.SelectedChoiceDoesNotBelongToQuestion(answer.QuestionId, answer.SelectedChoiceId);
        }

        return Result.Validated;
    }

    private static Result<Validated> ValidateTfAnswer(Question question, TfAnswer answer)
    {
        if (question is not Tf)
        {
            return QuizAttemptErrors.QuestionTypeMismatch(answer.QuestionId, "tf");
        }

        return Result.Validated;
    }
}
