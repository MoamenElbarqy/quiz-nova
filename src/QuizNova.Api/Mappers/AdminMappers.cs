using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

namespace QuizNova.Api.Mappers;

public static class AdminMappers
{
    public static CreateAdminCommand ToCommand(this CreateAdminRequest request) =>
        new(request.Name, request.Email, request.Password, request.PhoneNumber, request.Role);

    public static UpdateAdminCommand ToCommand(this UpdateAdminRequest request, Guid id) =>
        new(id, request.Name, request.Email, request.PhoneNumber);
}
