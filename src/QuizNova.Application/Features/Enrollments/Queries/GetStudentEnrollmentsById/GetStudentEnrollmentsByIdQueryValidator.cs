using FluentValidation;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;

public sealed class GetStudentEnrollmentsByIdQueryValidator : AbstractValidator<GetStudentEnrollmentsByIdQuery>
{
    public GetStudentEnrollmentsByIdQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
