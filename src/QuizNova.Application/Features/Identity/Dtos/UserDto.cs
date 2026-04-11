using System.Security.Claims;

namespace QuizNova.Application.Features.Identity.Dtos;

public sealed record UserDto(string UserId, string Name, string Role, IList<Claim> Claims);
