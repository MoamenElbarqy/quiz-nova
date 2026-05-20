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
            .MinimumLength(3).WithMessage("Title must not less than 3 characters.")
            .MaximumLength(30).WithMessage("Title must not exceed 30 characters.");

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndsAtUtc)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartsAtUtc).WithMessage("End time must be after start time.");
    }
}
