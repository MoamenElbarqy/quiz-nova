using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.EnrollStudentInCourse;

public sealed class EnrollStudentInCourseCommandValidator : AbstractValidator<EnrollStudentInCourseCommand>
{
    public EnrollStudentInCourseCommandValidator()
    {
        RuleFor(command => command.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");

        RuleFor(command => command.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");
    }
}
