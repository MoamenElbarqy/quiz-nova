using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;

public sealed class GetInstructorCoursesPerformanceQueryValidator : AbstractValidator<GetInstructorCoursesPerformanceQuery>
{
    public GetInstructorCoursesPerformanceQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}
