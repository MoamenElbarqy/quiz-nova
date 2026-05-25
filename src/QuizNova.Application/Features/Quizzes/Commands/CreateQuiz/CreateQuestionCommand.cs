using System.Text.Json.Serialization;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreateMcqCommand), "mcq")]
[JsonDerivedType(typeof(CreateTfCommand), "tf")]
[JsonDerivedType(typeof(CreateEssayCommand), "essay")]

public abstract record CreateQuestionCommand(
    string QuestionText,
    int Marks);
