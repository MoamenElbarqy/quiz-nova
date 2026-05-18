using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetEnrollmentsById;

public sealed class GetEnrollmentsByIdQueryValidator : AbstractValidator<GetEnrollmentsByIdQuery>
{
    public GetEnrollmentsByIdQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
