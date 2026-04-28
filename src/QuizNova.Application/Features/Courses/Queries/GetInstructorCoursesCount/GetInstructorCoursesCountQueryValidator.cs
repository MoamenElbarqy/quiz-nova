using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;

public sealed class GetInstructorCoursesCountQueryValidator : AbstractValidator<GetInstructorCoursesCountQuery>
{
    public GetInstructorCoursesCountQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}

