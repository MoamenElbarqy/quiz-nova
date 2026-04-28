using System.Security.Claims;

namespace QuizNova.Application.Features.Auth.DTOs;

public sealed record UserDto(string UserId, string Name, string Role, IList<Claim> Claims);
