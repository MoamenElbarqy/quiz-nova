namespace QuizNova.Application.Common.Interfaces;

using Domain.Entities.CourseChats;
using Domain.Entities.Courses;
using Domain.Entities.Enrollments;
using Domain.Entities.Identity;
using Domain.Entities.QuizAttempts;
using Domain.Entities.Quizzes;
using Domain.Entities.Users;

using MongoDB.Driver;

public interface IMongoDbContext
{
    IMongoCollection<Quiz> Quizzes { get; }

    IMongoCollection<QuizAttempt> QuizAttempts { get; }

    IMongoCollection<Course> Courses { get; }

    IMongoCollection<User> Users { get; }

    IMongoCollection<Enrollment> Enrollments { get; }

    IMongoCollection<CourseChatRoom> CourseChatRooms { get; }

    IMongoCollection<UserRefreshToken> UserRefreshTokens { get; }
}
