using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;

public sealed class GetInstructorQuizzesQueryValidator : AbstractValidator<GetInstructorQuizzesQuery>
{
    public GetInstructorQuizzesQueryValidator()
    {
        RuleFor(query => query.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}
