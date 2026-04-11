using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
    }
}
