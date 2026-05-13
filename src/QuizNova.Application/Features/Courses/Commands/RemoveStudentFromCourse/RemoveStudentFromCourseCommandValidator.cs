using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.RemoveStudentFromCourse;

public sealed class RemoveStudentFromCourseCommandValidator : AbstractValidator<RemoveStudentFromCourseCommand>
{
    public RemoveStudentFromCourseCommandValidator()
    {
        RuleFor(command => command.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");

        RuleFor(command => command.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");
    }
}
