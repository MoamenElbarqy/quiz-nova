using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.DeleteCourseById;

public sealed class DeleteCourseByIdCommandValidator : AbstractValidator<DeleteCourseByIdCommand>
{
    public DeleteCourseByIdCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");
    }
}
