using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.DeleteCourseById;

public sealed class DeleteCourseByIdValidator : AbstractValidator<DeleteCourseByIdCommand>
{
    public DeleteCourseByIdValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");
    }
}
