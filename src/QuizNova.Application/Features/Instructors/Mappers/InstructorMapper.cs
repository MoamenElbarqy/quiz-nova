using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Users.Mappers;
using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Application.Features.Instructors.Mappers;

public static class InstructorMapper
{
    public static InstructorDto ToInstructorDto(this Instructor instructor, int coursesCount, int quizzesCount)
    {
        return new InstructorDto(
            instructor.Id,
            instructor.PersonalInformation.ToDto(),
            coursesCount,
            quizzesCount);
    }
}
