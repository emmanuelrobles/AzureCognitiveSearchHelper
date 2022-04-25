namespace AzureCognitiveSearch.Abstractions;

public interface IPaginationResult<T>
{
    public long? Count { get; }
    public IAsyncEnumerable<T>? Items { get; }
}
