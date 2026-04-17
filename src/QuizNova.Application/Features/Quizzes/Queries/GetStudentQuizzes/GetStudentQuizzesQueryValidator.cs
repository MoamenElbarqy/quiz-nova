using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;

public sealed class GetStudentQuizzesQueryValidator : AbstractValidator<GetStudentQuizzesQuery>
{
    public GetStudentQuizzesQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty();
    }
}
