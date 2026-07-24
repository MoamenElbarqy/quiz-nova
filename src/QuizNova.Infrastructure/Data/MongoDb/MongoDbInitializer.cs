namespace QuizNova.Infrastructure.Data.MongoDb;

using Application.Common.Interfaces;

using Domain.Entities.QuizAttempts;
using Domain.Entities.Quizzes;

using MongoDB.Driver;

public static class MongoDbInitializer
{
    public static async Task InitializeIndexesAsync(IMongoDbContext mongoContext)
    {
        MongoDbClassMapper.RegisterClassMaps();

        await mongoContext.Quizzes.Indexes.CreateManyAsync([
            new CreateIndexModel<Quiz>(Builders<Quiz>.IndexKeys.Ascending(q => q.CourseId)),
            new CreateIndexModel<Quiz>(Builders<Quiz>.IndexKeys.Ascending(q => q.InstructorId)),
            new CreateIndexModel<Quiz>(Builders<Quiz>.IndexKeys.Combine(
                Builders<Quiz>.IndexKeys.Ascending(q => q.CourseId),
                Builders<Quiz>.IndexKeys.Descending(q => q.StartsAtUtc))),
            new CreateIndexModel<Quiz>(Builders<Quiz>.IndexKeys.Text(q => q.Title))
        ]);

        await mongoContext.QuizAttempts.Indexes.CreateManyAsync([
            new CreateIndexModel<QuizAttempt>(
                Builders<QuizAttempt>.IndexKeys.Combine(
                    Builders<QuizAttempt>.IndexKeys.Ascending(a => a.StudentId),
                    Builders<QuizAttempt>.IndexKeys.Ascending(a => a.QuizId)),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<QuizAttempt>(Builders<QuizAttempt>.IndexKeys.Ascending(a => a.QuizId)),
            new CreateIndexModel<QuizAttempt>(Builders<QuizAttempt>.IndexKeys.Ascending(a => a.StudentId))
        ]);
    }
}
