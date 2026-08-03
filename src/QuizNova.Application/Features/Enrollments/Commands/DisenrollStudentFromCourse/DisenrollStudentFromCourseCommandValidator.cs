using FluentValidation;

namespace QuizNova.Application.Features.Enrollments.Commands.DisenrollStudentFromCourse;

public sealed class DisenrollStudentFromCourseCommandValidator : AbstractValidator<DisenrollStudentFromCourseCommand>
{
    public DisenrollStudentFromCourseCommandValidator()
    {
        RuleFor(command => command.EnrollmentId)
            .NotEmpty()
            .WithMessage("Enrollment ID is required.");

        RuleFor(command => command.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");
    }
}
