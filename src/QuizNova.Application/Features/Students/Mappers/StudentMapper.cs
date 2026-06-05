using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Users.Mappers;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Students.Mappers;

public static class StudentMapper
{
    public static StudentDto ToStudentDto(this Student student, int enrolledCoursesCount)
    {
        return new StudentDto(
            student.Id,
            student.PersonalInformation.ToDto(),
            enrolledCoursesCount);
    }
}
