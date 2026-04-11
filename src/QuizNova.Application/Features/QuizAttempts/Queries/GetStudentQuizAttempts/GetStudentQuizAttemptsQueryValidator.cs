using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

public sealed class GetStudentQuizAttemptsQueryValidator : AbstractValidator<GetStudentQuizAttemptsQuery>
{
    public GetStudentQuizAttemptsQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty();
    }
}
