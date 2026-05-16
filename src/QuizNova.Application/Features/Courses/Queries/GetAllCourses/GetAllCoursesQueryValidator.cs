using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
{
    public GetAllCoursesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(query => query.InstructorId)
            .NotEqual(Guid.Empty).WithMessage("Instructor ID must be a valid GUID.")
            .When(query => query.InstructorId.HasValue);

        RuleFor(query => query.StudentId)
            .NotEqual(Guid.Empty).WithMessage("Student ID must be a valid GUID.")
            .When(query => query.StudentId.HasValue);

        RuleFor(query => query.EnrolledStudentsCount)
            .GreaterThanOrEqualTo(0).WithMessage("Enrolled students count cannot be negative.")
            .When(query => query.EnrolledStudentsCount.HasValue);

        RuleFor(query => query.QuizzesCount)
            .GreaterThanOrEqualTo(0).WithMessage("Quizzes count cannot be negative.")
            .When(query => query.QuizzesCount.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.")
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
