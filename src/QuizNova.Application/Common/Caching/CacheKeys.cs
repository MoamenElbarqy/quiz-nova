namespace QuizNova.Application.Common.Caching;

public static class CacheKeys
{
    public static string ById(string entity, object id) => $"{entity}:{id}";

    public static string All(string entity, params object?[] parameters) =>
        $"{entity}:all:{string.Join(":", parameters.Select(p => p?.ToString() ?? string.Empty))}";

    public static string ByRelation(string entity, string relation, object relationId, string? suffix = null) =>
        suffix is null ? $"{entity}:{relation}:{relationId}" : $"{entity}:{relation}:{relationId}:{suffix}";
}
