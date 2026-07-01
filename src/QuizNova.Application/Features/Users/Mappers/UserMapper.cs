using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Application.Features.Users.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id.ToString(),
            user.PersonalInformation.Name,
            user.UserRole.ToString());
    }
}
