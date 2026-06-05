using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.Features.Students.Commands.UpdateStudent;
using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Api.Mappers;

public static class StudentMappers
{
    public static CreateStudentCommand ToCommand(this CreateStudentRequest request) =>
        new(new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber), request.Password, request.Role);

    public static UpdateStudentCommand ToCommand(this UpdateStudentRequest request, Guid id) =>
        new(id, new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber));
}
