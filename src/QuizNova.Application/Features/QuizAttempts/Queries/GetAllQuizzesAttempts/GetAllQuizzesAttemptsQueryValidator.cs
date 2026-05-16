using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed class GetAllQuizzesAttemptsQueryValidator : AbstractValidator<GetAllQuizzesAttemptsQuery>
{
    public GetAllQuizzesAttemptsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(query => query.CorrectAnswers)
            .GreaterThanOrEqualTo(0).WithMessage("Correct answers count cannot be negative.")
            .When(query => query.CorrectAnswers.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.")
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
