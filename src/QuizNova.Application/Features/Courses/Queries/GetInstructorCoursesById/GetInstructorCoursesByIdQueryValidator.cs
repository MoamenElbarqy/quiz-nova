using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;

public sealed class GetInstructorCoursesByIdQueryValidator : AbstractValidator<GetInstructorCoursesByIdQuery>
{
    public GetInstructorCoursesByIdQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}

