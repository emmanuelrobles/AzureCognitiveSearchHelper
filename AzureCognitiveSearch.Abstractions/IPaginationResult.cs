namespace AzureCognitiveSearch.Abstractions;

/// <summary>
/// Base query result
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IPaginationResult<out TResult>
{
    /// <summary>
    /// How many total items
    /// </summary>
    public long? Count { get; }
    
    /// <summary>
    /// Stream of items
    /// </summary>
    public IAsyncEnumerable<TResult>? Items { get; }
}
