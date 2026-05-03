using FluentValidation;

namespace QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryValidator : AbstractValidator<GetAllInstructorsQuery>
{
    public GetAllInstructorsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
            .When(query => !string.IsNullOrEmpty(query.SearchTerm));

        RuleFor(query => query.CoursesCount)
            .GreaterThanOrEqualTo(0).WithMessage("Courses count must be non-negative.")
            .When(query => query.CoursesCount.HasValue);

        RuleFor(query => query.QuizzesCount)
            .GreaterThanOrEqualTo(0).WithMessage("Quizzes count must be non-negative.")
            .When(query => query.QuizzesCount.HasValue);
    }
}
