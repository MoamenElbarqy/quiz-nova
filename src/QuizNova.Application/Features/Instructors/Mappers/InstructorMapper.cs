using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Mappers;

public static class InstructorMapper
{
    public static InstructorDto ToInstructorDto(this Instructor instructor, int coursesCount, int quizzesCount)
    {
        return new InstructorDto(
            instructor.Id,
            instructor.PersonalInformation.Name,
            instructor.PersonalInformation.Email,
            instructor.PersonalInformation.Password,
            instructor.PersonalInformation.PhoneNumber,
            coursesCount,
            quizzesCount);
    }
}
