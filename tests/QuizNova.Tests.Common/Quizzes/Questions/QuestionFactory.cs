using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;

namespace QuizNova.Tests.Common.Quizzes.Questions;

public static class QuestionFactory
{
    public static Result<Tf> CreateTfQuestion(
        Guid? id = null,
        Guid? quizId = null,
        string questionText = "Is this a true statement?",
        bool correctChoice = true,
        int displayOrder = 1,
        int marks = 5)
    {
        return Tf.Create(
            id ?? Guid.NewGuid(),
            quizId ?? Guid.NewGuid(),
            questionText,
            correctChoice,
            displayOrder,
            marks);
    }

    public static Result<Mcq> CreateMcqQuestion(
        Guid? id = null,
        Guid? quizId = null,
        string questionText = "Which option is correct?",
        Guid? correctChoiceId = null,
        int displayOrder = 1,
        int marks = 5,
        List<Choice>? choices = null)
    {
        var mcqId = id ?? Guid.NewGuid();
        if (choices == null)
        {
            var choice1Id = correctChoiceId ?? Guid.NewGuid();
            var choice2Id = Guid.NewGuid();
            var choice1 = Choice.Create(choice1Id, mcqId, "Option A (Correct)", 1).Value;
            var choice2 = Choice.Create(choice2Id, mcqId, "Option B", 2).Value;
            choices = [choice1, choice2];
            correctChoiceId = choice1Id;
        }
        else if (correctChoiceId == null || correctChoiceId == Guid.Empty)
        {
            correctChoiceId = choices.FirstOrDefault()?.Id ?? Guid.NewGuid();
        }

        return Mcq.Create(
            mcqId,
            quizId ?? Guid.NewGuid(),
            questionText,
            correctChoiceId.Value,
            displayOrder,
            marks,
            choices);
    }

}
