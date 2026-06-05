using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;
using QuizNova.Application.Features.Users.DTOs;

namespace QuizNova.Api.Mappers;

public static class InstructorMappers
{
    public static CreateInstructorCommand ToCommand(this CreateInstructorRequest request) =>
        new(new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber), request.Password, request.Role);

    public static UpdateInstructorCommand ToCommand(this UpdateInstructorRequest request, Guid id) =>
        new(id, new PersonalInformationDto(request.Name, request.Email, request.PhoneNumber));
}
