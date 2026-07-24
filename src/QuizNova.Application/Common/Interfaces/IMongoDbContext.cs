namespace QuizNova.Application.Common.Interfaces;

using Domain.Entities.QuizAttempts;
using Domain.Entities.Quizzes;

using MongoDB.Driver;

public interface IMongoDbContext
{
    IMongoCollection<Quiz> Quizzes { get; }

    IMongoCollection<QuizAttempt> QuizAttempts { get; }
}
