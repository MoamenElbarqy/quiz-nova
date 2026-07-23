using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Common;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Course> Courses { get; }

    DbSet<QuizAttempt> QuizAttempts { get; }

    DbSet<Quiz> Quizzes { get; }

    DbSet<Question> Questions { get; }

    DbSet<QuestionAnswer> QuestionAnswers { get; }

    DbSet<ManuallyGradedAnswers> ManuallyGradedAnswers { get; }

    DbSet<EssayAnswer> EssayAnswers { get; }

    DbSet<Choice> Choices { get; }

    DbSet<User> Users { get; }

    DbSet<Instructor> Instructors { get; }

    DbSet<Student> Students { get; }

    DbSet<Admin> Admins { get; }

    DbSet<Enrollment> Enrollments { get; }

    DbSet<CourseChatRoom> CourseChatRooms { get; }

    DbSet<Message> CourseChatRoomMessages { get; }

    DbSet<Reaction> Reactions { get; }

    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
