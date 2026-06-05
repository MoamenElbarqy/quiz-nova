using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Users.Mappers;

public static class PersonalInformationMapper
{
    public static PersonalInformationDto ToDto(this PersonalInformation personalInformation)
    {
        return new PersonalInformationDto(
            personalInformation.Name,
            personalInformation.Email,
            personalInformation.PhoneNumber);
    }
}
