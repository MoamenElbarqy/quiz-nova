using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;

public sealed class GetInstructorCoursesQueryValidator : AbstractValidator<GetInstructorCoursesQuery>
{
    public GetInstructorCoursesQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}
