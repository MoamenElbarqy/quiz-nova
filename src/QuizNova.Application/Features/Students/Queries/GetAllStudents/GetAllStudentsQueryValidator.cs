using FluentValidation;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryValidator : AbstractValidator<GetAllStudentsQuery>
{
    public GetAllStudentsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(query => query.EnrolledCoursesCount)
            .GreaterThanOrEqualTo(0).WithMessage("Enrolled courses count cannot be negative.")
            .When(query => query.EnrolledCoursesCount.HasValue);

        RuleFor(query => query.CourseId)
            .NotEqual(Guid.Empty).WithMessage("Course ID must be a valid GUID.")
            .When(query => query.CourseId.HasValue);

        RuleFor(query => query.IsEnrolledInCourse)
            .NotNull().WithMessage("Enrollment filter is required when course ID is provided.")
            .When(query => query.CourseId.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.")
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
