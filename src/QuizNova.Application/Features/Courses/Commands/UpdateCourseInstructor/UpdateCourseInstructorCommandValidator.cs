using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;

public sealed class UpdateCourseInstructorCommandValidator : AbstractValidator<UpdateCourseInstructorCommand>
{
    public UpdateCourseInstructorCommandValidator()
    {
        RuleFor(command => command.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");

        RuleFor(command => command.InstructorId)
            .NotEqual(Guid.Empty)
            .When(command => command.InstructorId.HasValue)
            .WithMessage("Instructor ID must be valid when provided.");
    }
}
