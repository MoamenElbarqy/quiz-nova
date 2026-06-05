using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Admins.Commands.UpdateAdmin;
using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Api.Mappers;

public static class AdminMappers
{
    public static CreateAdminCommand ToCommand(this CreateAdminRequest request) =>
        new(new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber), request.Password, request.Role);

    public static UpdateAdminCommand ToCommand(this UpdateAdminRequest request, Guid id) =>
        new(id, new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber));
}
