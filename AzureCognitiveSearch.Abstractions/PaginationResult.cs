namespace AzureCognitiveSearch.Abstractions;

public class PaginationResult<T>
{
    public long? Count { get; init; }
    public IAsyncEnumerable<T> Items { get; init; }
}
