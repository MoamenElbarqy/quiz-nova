using FluentValidation;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryValidator : AbstractValidator<GetAllStudentsQuery>
{
    public GetAllStudentsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(query => query.EnrolledCoursesCount)
            .GreaterThanOrEqualTo(0)
            .When(query => query.EnrolledCoursesCount.HasValue);

        RuleFor(query => query.CourseId)
            .NotEqual(Guid.Empty)
            .When(query => query.CourseId.HasValue)
            .WithMessage("Course ID must be valid when provided.");

        RuleFor(query => query.IsEnrolledInCourse)
            .NotNull()
            .When(query => query.CourseId.HasValue)
            .WithMessage("Enrollment filter is required when course ID is provided.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
