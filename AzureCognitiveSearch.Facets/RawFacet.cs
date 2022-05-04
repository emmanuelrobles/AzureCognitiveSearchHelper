using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Facets;

/// <summary>
/// Creates a facet from the given string
/// </summary>
public class RawFacet : IFacet
{
    private readonly string _facetQuery;

    public RawFacet(string facetQuery)
    {
        _facetQuery = facetQuery;
    }
    
    public string BuildFacet() => _facetQuery;
}
