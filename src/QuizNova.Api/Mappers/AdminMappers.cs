using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Api.Mappers;

public static class AdminMappers
{
    public static CreateAdminCommand ToCommand(this CreateAdminRequest request) =>
        new(new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber), request.Password, request.Role);
}
