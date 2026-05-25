using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestionManually;

public sealed class GradeQuestionManuallyCommandValidator : AbstractValidator<GradeQuestionManuallyCommand>
{
    public GradeQuestionManuallyCommandValidator()
    {
        RuleFor(command => command.AnswerId)
            .NotEmpty()
            .WithMessage("Answer ID is required.");

        RuleFor(command => command.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score must be greater than or equal to zero.");
    }
}
