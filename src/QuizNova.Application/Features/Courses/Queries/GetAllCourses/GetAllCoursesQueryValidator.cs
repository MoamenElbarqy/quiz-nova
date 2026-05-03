using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
{
    public GetAllCoursesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(query => query.InstructorId)
            .NotEqual(Guid.Empty)
            .When(query => query.InstructorId.HasValue);

        RuleFor(query => query.StudentId)
            .NotEqual(Guid.Empty)
            .When(query => query.StudentId.HasValue);

        RuleFor(query => query.EnrolledStudentsCount)
            .GreaterThanOrEqualTo(0)
            .When(query => query.EnrolledStudentsCount.HasValue);

        RuleFor(query => query.QuizzesCount)
            .GreaterThanOrEqualTo(0)
            .When(query => query.QuizzesCount.HasValue);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
