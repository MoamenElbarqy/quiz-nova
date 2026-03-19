namespace QuizNova.Application.Features.Identity.Dtos;

public sealed record AuthDto(
    TokenDto Token,
    UserDto User);
