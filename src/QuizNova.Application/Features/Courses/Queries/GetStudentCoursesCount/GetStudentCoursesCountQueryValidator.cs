using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetEnrollmentsCount;

public sealed class GetEnrollmentsCountQueryValidator : AbstractValidator<GetEnrollmentsCountQuery>
{
    public GetEnrollmentsCountQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}

