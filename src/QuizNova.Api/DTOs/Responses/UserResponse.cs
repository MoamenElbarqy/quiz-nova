namespace QuizNova.Api.DTOs.Responses;

public sealed class UserResponse
{
    public string UserId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public IReadOnlyList<ClaimResponse> Claims { get; init; } = [];
}
