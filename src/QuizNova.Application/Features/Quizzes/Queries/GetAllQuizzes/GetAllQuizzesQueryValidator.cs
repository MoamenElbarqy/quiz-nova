using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed class GetAllQuizzesQueryValidator : AbstractValidator<GetAllQuizzesQuery>
{
    public GetAllQuizzesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(query => query.Marks)
            .GreaterThanOrEqualTo(0).WithMessage("Marks count cannot be negative.")
            .When(query => query.Marks.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.")
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
