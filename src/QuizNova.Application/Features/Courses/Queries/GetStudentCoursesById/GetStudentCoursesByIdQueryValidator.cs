using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesById;

public sealed class GetStudentCoursesByIdQueryValidator : AbstractValidator<GetStudentCoursesByIdQuery>
{
    public GetStudentCoursesByIdQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
