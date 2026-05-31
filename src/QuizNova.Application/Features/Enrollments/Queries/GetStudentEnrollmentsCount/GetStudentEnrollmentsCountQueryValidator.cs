using FluentValidation;

namespace QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;

public sealed class GetStudentEnrollmentsCountQueryValidator : AbstractValidator<GetStudentEnrollmentsCountQuery>
{
    public GetStudentEnrollmentsCountQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
