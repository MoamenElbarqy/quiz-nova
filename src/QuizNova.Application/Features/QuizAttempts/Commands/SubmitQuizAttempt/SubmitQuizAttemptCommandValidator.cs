using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptCommandValidator()
    {
        RuleFor(command => command.QuizAttemptId).NotEmpty();
        RuleFor(command => command.StudentId).NotEmpty();
        RuleFor(command => command.QuizId).NotEmpty();
        RuleFor(command => command.StartedAt).NotEqual(default(DateTimeOffset));
        RuleFor(command => command.SubmittedAt).NotEqual(default(DateTimeOffset));

        RuleFor(command => command)
            .Must(command => command.StartedAt < command.SubmittedAt)
            .WithMessage("StartedAt must be before SubmittedAt.");

        RuleFor(command => command.QuestionAnswers)
            .NotEmpty();

        RuleFor(command => command.QuestionAnswers)
            .Must(answers => answers.Select(answer => answer.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Duplicate question answers are not allowed.");
    }
}
