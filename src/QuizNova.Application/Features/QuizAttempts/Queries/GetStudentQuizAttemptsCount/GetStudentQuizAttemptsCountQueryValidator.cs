using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

public sealed class GetStudentQuizAttemptsCountQueryValidator : AbstractValidator<GetStudentQuizAttemptsCountQuery>
{
    public GetStudentQuizAttemptsCountQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}

