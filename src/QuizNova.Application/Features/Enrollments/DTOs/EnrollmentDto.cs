namespace QuizNova.Application.Features.Enrollments.DTOs;

public sealed record EnrollmentInstructorDto(
    Guid InstructorId,
    string Name);

public sealed record EnrollmentStudentDto(
    Guid StudentId,
    string Name,
    int QuizzesTaken);

public sealed record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    string CourseName,
    EnrollmentInstructorDto Instructor,
    EnrollmentStudentDto Student,
    DateTimeOffset EnrolledOnUtc);
