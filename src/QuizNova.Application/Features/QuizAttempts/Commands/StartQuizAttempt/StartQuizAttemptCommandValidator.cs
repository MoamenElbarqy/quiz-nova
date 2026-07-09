using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.StartQuizAttempt;

public sealed class StartQuizAttemptCommandValidator : AbstractValidator<StartQuizAttemptCommand>
{
    public StartQuizAttemptCommandValidator()
    {
        RuleFor(command => command.QuizId).NotEmpty().WithMessage("Quiz ID is required.");
    }
}
