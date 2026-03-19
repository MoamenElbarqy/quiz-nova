using QuizNova.Api.DTOs.Responses;
using QuizNova.Application.Features.Identity.Dtos;

namespace QuizNova.Api.Mappers;

public static class AuthMapper
{
    public static LoginResponse ToLoginResponse(this AuthDto authDto, string message)
    {
        return new LoginResponse
        {
            Message = message,
            Token = new TokenResponse
            {
                AccessToken = authDto.Token.AccessToken,
                RefreshToken = authDto.Token.RefreshToken,
                ExpiresOnUtc = authDto.Token.ExpiresOnUtc,
            },
            User = new UserResponse
            {
                UserId = authDto.User.UserId,
                Name = authDto.User.Name,
                Role = authDto.User.Role,
                Claims = authDto.User.Claims
                    .Select(claim => new ClaimResponse
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                    })
                    .ToArray(),
            },
        };
    }
}
