using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public sealed class GetPendingManualAnswersQueryValidator : AbstractValidator<GetPendingManualAnswersQuery>
{
    public GetPendingManualAnswersQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
    }
}
