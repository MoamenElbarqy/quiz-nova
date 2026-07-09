using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.CompleteQuizAttempt;

public sealed class CompleteQuizAttemptCommandValidator : AbstractValidator<CompleteQuizAttemptCommand>
{
    public CompleteQuizAttemptCommandValidator()
    {
        RuleFor(command => command.AttemptId).NotEmpty().WithMessage("Attempt ID is required.");
        RuleFor(command => command.SubmittedAt).NotEqual(default(DateTimeOffset))
            .WithMessage("Submitted at time is required.");
    }
}
