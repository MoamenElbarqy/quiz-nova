using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Application.Features.Admins.Mappers;

public static class AdminMapper
{
    public static AdminDto ToAdminDto(this Admin admin)
    {
        return new AdminDto(
            admin.Id,
            admin.PersonalInformation.Name,
            admin.PersonalInformation.Email,
            admin.PersonalInformation.Password,
            admin.PersonalInformation.PhoneNumber);
    }
}
