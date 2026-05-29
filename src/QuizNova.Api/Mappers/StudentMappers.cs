using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.Features.Students.Commands.UpdateStudent;

namespace QuizNova.Api.Mappers;

public static class StudentMappers
{
    public static CreateStudentCommand ToCommand(this CreateStudentRequest request) =>
        new(request.Name, request.Email, request.Password, request.PhoneNumber, request.Role);

    public static UpdateStudentCommand ToCommand(this UpdateStudentRequest request, Guid id) =>
        new(id, request.Name, request.Email, request.PhoneNumber);
}
