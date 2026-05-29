using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

namespace QuizNova.Api.Mappers;

public static class InstructorMappers
{
    public static CreateInstructorCommand ToCommand(this CreateInstructorRequest request) =>
        new(request.Name, request.Email, request.Password, request.PhoneNumber, request.Role);

    public static UpdateInstructorCommand ToCommand(this UpdateInstructorRequest request, Guid id) =>
        new(id, request.Name, request.Email, request.PhoneNumber);
}
