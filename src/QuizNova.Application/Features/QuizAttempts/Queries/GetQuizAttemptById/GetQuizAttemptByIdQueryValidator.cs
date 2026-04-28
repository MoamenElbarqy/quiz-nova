using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;

public sealed class GetQuizAttemptByIdQueryValidator : AbstractValidator<GetQuizAttemptByIdQuery>
{
    public GetQuizAttemptByIdQueryValidator()
    {
        RuleFor(x => x.QuizAttemptId)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required.");
    }
}
