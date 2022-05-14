using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Facets;

/// <summary>
/// Creates a facet from the given string
/// </summary>
public class RawFacetBuilder : IFacetBuilder
{
    private readonly string _facetQuery;

    public RawFacetBuilder(string facetQuery)
    {
        _facetQuery = facetQuery;
    }
    
    public string BuildFacet() => _facetQuery;
}
