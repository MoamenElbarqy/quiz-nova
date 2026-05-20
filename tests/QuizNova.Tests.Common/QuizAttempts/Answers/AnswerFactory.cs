using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Tests.Common.Quizzes.Questions;

namespace QuizNova.Tests.Common.QuizAttempts.Answers;

public static class AnswerFactory
{
    public static Result<McqAnswer> CreateMcqAnswer(
        Guid? id = null,
        Guid? studentId = null,
        Guid? questionId = null,
        Guid? quizAttemptId = null,
        Guid? selectedChoiceId = null,
        Mcq? question = null,
        bool isCorrect = true)
    {
        var qId = questionId ?? Guid.NewGuid();
        var choiceId = selectedChoiceId ?? Guid.NewGuid();

        if (question == null)
        {
            var choice1 = Choice.Create(choiceId, qId, "Option A (Correct)", 1).Value;
            var choice2 = Choice.Create(Guid.NewGuid(), qId, "Option B", 2).Value;
            question = QuestionFactory.CreateMcqQuestion(
                id: qId,
                correctChoiceId: choiceId,
                choices: [choice1, choice2]).Value;
        }

        return McqAnswer.Create(
            id ?? Guid.NewGuid(),
            studentId ?? Guid.NewGuid(),
            qId,
            quizAttemptId ?? Guid.NewGuid(),
            choiceId,
            question,
            isCorrect);
    }

    public static Result<TfAnswer> CreateTfAnswer(
        Guid? id = null,
        Guid? studentId = null,
        Guid? questionId = null,
        Guid? quizAttemptId = null,
        bool studentChoice = true,
        bool isCorrect = true)
    {
        return TfAnswer.Create(
            id ?? Guid.NewGuid(),
            studentId ?? Guid.NewGuid(),
            questionId ?? Guid.NewGuid(),
            quizAttemptId ?? Guid.NewGuid(),
            studentChoice,
            isCorrect);
    }
}
