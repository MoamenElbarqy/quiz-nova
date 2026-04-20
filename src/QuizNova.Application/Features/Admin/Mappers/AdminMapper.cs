using QuizNova.Application.Features.Admin.DTOs;

using DomainAdmin = QuizNova.Domain.Entities.Users.Admin;

namespace QuizNova.Application.Features.Admin.Mappers;

public static class AdminMapper
{
    public static AdminDto ToAdminDto(this DomainAdmin admin)
    {
        return new AdminDto(
            admin.Id,
            admin.PersonalInformation.Name,
            admin.PersonalInformation.Email,
            admin.PersonalInformation.Password,
            admin.PersonalInformation.PhoneNumber);
    }
}
