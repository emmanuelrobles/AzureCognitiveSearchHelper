namespace AzureCognitiveSearch.Abstractions;

public interface IPaginationResult<T>
{
    public long? Count { get; init; }
    public IAsyncEnumerable<T> Items { get; init; }
}
