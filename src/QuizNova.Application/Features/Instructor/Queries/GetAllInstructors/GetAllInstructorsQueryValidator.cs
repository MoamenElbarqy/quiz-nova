using FluentValidation;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryValidator : AbstractValidator<GetAllInstructorsQuery>
{
    public GetAllInstructorsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(query => query.CoursesCount)
            .GreaterThanOrEqualTo(0)
            .When(query => query.CoursesCount.HasValue);

        RuleFor(query => query.QuizzesCount)
            .GreaterThanOrEqualTo(0)
            .When(query => query.QuizzesCount.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
