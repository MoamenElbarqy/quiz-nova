using System.Linq.Expressions;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Application.Common.MongoDb;

public static class MongoDbQueryHelper
{
    public static async Task<QuizAttempt?> GetAttemptWithQuizAsync(
        this IMongoCollection<QuizAttempt> collection,
        Expression<Func<QuizAttempt, bool>> filter,
        CancellationToken ct)
    {
        var result = await collection
            .Aggregate()
            .Match(filter)
            .Lookup("quizzes", "QuizId", "_id", "Quiz")
            .Unwind("Quiz", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .FirstOrDefaultAsync(ct);

        return result is null ? null : BsonSerializer.Deserialize<QuizAttempt>(result);
    }
}
