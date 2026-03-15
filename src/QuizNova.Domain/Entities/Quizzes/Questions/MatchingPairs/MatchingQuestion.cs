using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs.CorrectAnswerPairs;
using QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs.LeftChoices;
using QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs.RightChoices;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public class MatchingQuestion : Question
{
    private readonly List<MatchingLeftChoice> _leftChoices;

    private readonly List<MatchingRightChoice> _rightChoices;

    private readonly List<CorrectAnswerPair> _correctPairs;

    public IEnumerable<MatchingLeftChoice> LeftChoices => _leftChoices.AsReadOnly();

    public IEnumerable<MatchingRightChoice> RightChoices => _rightChoices.AsReadOnly();

    public IEnumerable<CorrectAnswerPair> CorrectPairs => _correctPairs.AsReadOnly();

    private MatchingQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks,
        List<MatchingLeftChoice> leftChoices,
        List<MatchingRightChoice> rightChoices,
        List<CorrectAnswerPair> correctPairs)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        _leftChoices = leftChoices;
        _rightChoices = rightChoices;
        _correctPairs = correctPairs;
    }

    public static Result<MatchingQuestion> Create(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks,
        List<MatchingLeftChoice> leftChoices,
        List<MatchingRightChoice> rightChoices,
        List<CorrectAnswerPair> correctPairs)
    {
        var validationError = ValidateCommon(
            quizId,
            questionText,
            displayOrder,
            marks);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new MatchingQuestion(id, quizId, questionText, displayOrder, marks, leftChoices, rightChoices, correctPairs);
    }
}
