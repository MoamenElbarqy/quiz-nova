namespace QuizNova.Infrastructure.Data.MongoDb;

using Domain.Entities.QuizAttempts;
using Domain.Entities.QuizAttempts.Answers.Base;
using Domain.Entities.Quizzes;
using Domain.Entities.Quizzes.Questions.Base;

using MongoDB.Bson.Serialization;

public static class MongoDbClassMapper
{
    private static readonly object RegistrationLock = new();
    private static bool _registered;

    public static void RegisterClassMaps()
    {
        if (_registered)
        {
            return;
        }

        lock (RegistrationLock)
        {
            if (_registered)
            {
                return;
            }

            RegisterHierarchy<Question>();
            RegisterHierarchy<QuestionAnswer>();
            RegisterQuizClassMap();
            RegisterQuizAttemptClassMap();
            _registered = true;
        }
    }

    private static void RegisterQuizClassMap()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Quiz)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Quiz>(cm =>
        {
            cm.AutoMap();
        });
    }

    private static void RegisterQuizAttemptClassMap()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(QuizAttempt)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<QuizAttempt>(cm =>
        {
            cm.AutoMap();
        });
    }

    private static void RegisterHierarchy<TRoot>()
        where TRoot : class
    {
        var rootType = typeof(TRoot);

        if (BsonClassMap.IsClassMapRegistered(rootType))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<TRoot>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        var derivedTypes = rootType.Assembly.GetTypes()
            .Where(t => rootType.IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in derivedTypes)
        {
            BsonClassMap.LookupClassMap(type);
        }
    }
}
