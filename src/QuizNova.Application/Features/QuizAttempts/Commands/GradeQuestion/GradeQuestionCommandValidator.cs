using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;

public sealed class GradeQuestionCommandValidator : AbstractValidator<GradeQuestionCommand>
{
    public GradeQuestionCommandValidator()
    {
        RuleFor(command => command.AnswerId)
            .NotEmpty()
            .WithMessage("Answer ID is required.");

        RuleFor(command => command.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score must be greater than or equal to zero.");

        When(command => command.Feedback is not null, () =>
        {
            RuleFor(command => command.Feedback)
                .MinimumLength(3).WithMessage("Feedback must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Feedback must not exceed 200 characters.");
        });
    }
}
