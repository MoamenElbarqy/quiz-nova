using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;

public sealed class GetAllQuizzesQueryValidator : AbstractValidator<GetAllQuizzesQuery>
{
    public GetAllQuizzesQueryValidator()
    {
    }
}
