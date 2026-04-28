using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;

public sealed class GetInstructorQuizzesCountQueryValidator : AbstractValidator<GetInstructorQuizzesCountQuery>
{
    public GetInstructorQuizzesCountQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}

