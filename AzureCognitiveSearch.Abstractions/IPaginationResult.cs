namespace AzureCognitiveSearch.Abstractions;

public interface IPaginationResult<out TResult>
{
    public long? Count { get; }
    public IAsyncEnumerable<TResult>? Items { get; }
}
