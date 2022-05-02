using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Context.FromNugetPackage;

internal struct ValuePaginationResult<TResult> : IPaginationResult<TResult>
{
    public long? Count { get; init; }
    public IAsyncEnumerable<TResult>? Items { get; init; }
}
