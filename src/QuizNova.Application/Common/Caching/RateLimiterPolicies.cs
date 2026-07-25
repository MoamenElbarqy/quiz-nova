namespace QuizNova.Application.Common.Caching;

public static class RateLimiterPolicies
{
    public const string Global = "Global";
    public const string SubmitQuiz = "SubmitQuiz";
    public const string Auth = "Auth";
}
