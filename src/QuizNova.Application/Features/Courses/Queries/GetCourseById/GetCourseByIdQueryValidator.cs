using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");
    }
}
