using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace QuizNova.Api.OpenApi.Transformers;

internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken ct)
    {
        // this is will be the version name due the way we calling the add open api service
        var version = context.DocumentName;

        document.Info.Version = version;
        document.Info.Title = $"QuizNova API {version}";

        return Task.CompletedTask;
    }
}