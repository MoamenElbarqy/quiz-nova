using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public sealed class GetAllQuizzesAttemptsQueryValidator : AbstractValidator<GetAllQuizzesAttemptsQuery>
{
    public GetAllQuizzesAttemptsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(query => query.CorrectAnswers)
            .GreaterThanOrEqualTo(0)
            .When(query => query.CorrectAnswers.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
