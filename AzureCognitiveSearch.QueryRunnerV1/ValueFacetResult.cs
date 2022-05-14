using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.QueryRunnerV1;

public struct ValueFacetResult : IFacetResult
{
    public string Value { get; set; }
    public long? Count { get; set; }
}
