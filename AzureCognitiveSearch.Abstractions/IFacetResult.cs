namespace AzureCognitiveSearch.Abstractions;

public interface IFacetResult
{
    public string Value { get; set; }
    public long? Count { get; set; }
}
