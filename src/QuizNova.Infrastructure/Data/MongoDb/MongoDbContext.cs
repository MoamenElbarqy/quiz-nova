namespace QuizNova.Infrastructure.Data.MongoDb;

using Application.Common.Interfaces;

using Domain.Entities.CourseChats;
using Domain.Entities.Courses;
using Domain.Entities.Enrollments;
using Domain.Entities.Identity;
using Domain.Entities.QuizAttempts;
using Domain.Entities.Quizzes;
using Domain.Entities.Users;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

using Settings;

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings, IHostEnvironment environment)
    {
        MongoDbClassMapper.RegisterClassMaps();

        var clientSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);

        clientSettings.MaxConnectionPoolSize = settings.Value.MaxConnectionPoolSize;
        clientSettings.MinConnectionPoolSize = settings.Value.MinConnectionPoolSize;
        clientSettings.MaxConnecting = settings.Value.MaxConnecting;
        clientSettings.WaitQueueTimeout = TimeSpan.FromMinutes(settings.Value.WaitQueueTimeoutMinutes);

        if (!environment.IsEnvironment("Testing"))
        {
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        }

        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Quiz> Quizzes => _database.GetCollection<Quiz>("quizzes");

    public IMongoCollection<QuizAttempt> QuizAttempts => _database.GetCollection<QuizAttempt>("quiz_attempts");

    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");

    public IMongoCollection<Enrollment> Enrollments => _database.GetCollection<Enrollment>("enrollments");

    public IMongoCollection<CourseChatRoom> CourseChatRooms => _database.GetCollection<CourseChatRoom>("course_chat_rooms");

    public IMongoCollection<UserRefreshToken> UserRefreshTokens => _database.GetCollection<UserRefreshToken>("user_refresh_tokens");
}
