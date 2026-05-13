namespace QuizNova.Api.DTOs.Requests;

public sealed record UpdateQuizMetadataRequest(
    string Title,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc);
