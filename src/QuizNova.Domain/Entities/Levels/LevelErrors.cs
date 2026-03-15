using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Levels;

public static class LevelErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Level_Name_Required", "Level name is required.");
}
