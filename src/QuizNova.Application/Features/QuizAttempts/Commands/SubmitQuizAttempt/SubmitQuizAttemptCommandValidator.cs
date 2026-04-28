using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptCommandValidator()
    {
        RuleFor(command => command.QuizAttemptId).NotEmpty().WithMessage("Quiz attempt ID is required.");
        RuleFor(command => command.StudentId).NotEmpty().WithMessage("Student ID is required.");
        RuleFor(command => command.QuizId).NotEmpty().WithMessage("Quiz ID is required.");
        RuleFor(command => command.StartedAt).NotEqual(default(DateTimeOffset)).WithMessage("Started at date is required.");
        RuleFor(command => command.SubmittedAt).NotEqual(default(DateTimeOffset)).WithMessage("Submitted at date is required.");

        RuleFor(command => command)
            .Must(command => command.StartedAt < command.SubmittedAt)
            .WithMessage("StartedAt must be before SubmittedAt.");

        RuleFor(command => command.QuestionAnswers)
            .NotEmpty().WithMessage("Question answers are required.");

        RuleFor(command => command.QuestionAnswers)
            .Must(answers => answers.Select(answer => answer.AnswerId).Distinct().Count() == answers.Count)
            .WithMessage("Duplicate answer IDs are not allowed.");

        RuleFor(command => command.QuestionAnswers)
            .Must(answers => answers.Select(answer => answer.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Duplicate question answers are not allowed.");
    }
}
