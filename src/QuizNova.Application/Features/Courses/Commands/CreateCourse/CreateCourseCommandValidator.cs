using FluentValidation;

namespace QuizNova.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Course ID is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Course name is required.");

        RuleFor(command => command.InstructorId)
            .NotEqual(Guid.Empty)
            .When(command => command.InstructorId.HasValue)
            .WithMessage("Instructor ID must be valid when provided.");

        RuleFor(command => command.MinimumPassingMarks)
            .GreaterThan(0)
            .WithMessage("Minimum passing marks must be greater than zero.");

        RuleFor(command => command.MaximumMarks)
            .GreaterThan(0)
            .WithMessage("Maximum marks must be greater than zero.");

        RuleFor(command => command)
            .Must(command => command.MinimumPassingMarks <= command.MaximumMarks)
            .WithMessage("Minimum passing marks cannot exceed maximum marks.");
    }
}
