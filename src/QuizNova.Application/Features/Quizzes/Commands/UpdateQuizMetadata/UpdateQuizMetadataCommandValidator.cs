using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;

public sealed class UpdateQuizMetadataCommandValidator : AbstractValidator<UpdateQuizMetadataCommand>
{
    public UpdateQuizMetadataCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndsAtUtc)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartsAtUtc).WithMessage("End time must be after start time.");
    }
}
