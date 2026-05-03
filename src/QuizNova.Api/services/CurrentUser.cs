using System.Security.Claims;

using QuizNova.Application.Common.Interfaces;

namespace QuizNova.Api.services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
