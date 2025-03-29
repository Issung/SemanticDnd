using Refit;

namespace DndTest;

public interface IOllamaClient
{
    [Post("/api/embed")]
    Task<EmbeddingsResponse> Embeddings(EmbeddingsRequest request);
}

public record EmbeddingsRequest(
    string Model,
    string Input
);

public record EmbeddingsResponse(
    string Model,
    IReadOnlyList<IReadOnlyList<float>> Embeddings,
    long TotalDuration,
    int LoadDuration,
    int PromptEvalCount
);