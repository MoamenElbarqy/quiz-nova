using FluentValidation;

namespace QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;

public sealed class RemoveStudentFromCourseCommandValidator : AbstractValidator<RemoveStudentFromCourseCommand>
{
    public RemoveStudentFromCourseCommandValidator()
    {
        RuleFor(command => command.EnrollmentId)
            .NotEmpty()
            .WithMessage("Enrollment ID is required.");

        RuleFor(command => command.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");
    }
}
