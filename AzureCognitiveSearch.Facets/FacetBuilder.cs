using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Facets;

public static class FacetBuilder
{
    // countFacet
    public static IFacetBuilder RawCountFacet(string propName, uint count)
        => new RawFacetBuilder($"{propName},count:{count}");

    // value facets
    public static IFacetBuilder RawValueFacet(string propName, IEnumerable<int> values)
        => RawValueFacet(propName, values.Select(i => i.ToString()));
    
    public static IFacetBuilder RawValueFacet(string propName, IEnumerable<DateTimeOffset> values)
        => RawValueFacet(propName, values.Select(dt => dt.ToString()));

    // base value facet
    private static IFacetBuilder RawValueFacet(string propName, IEnumerable<string> values)
        => new RawFacetBuilder($"{propName},values:{string.Join('|', values)}");

    // interval facets
    public static IFacetBuilder RawIntervalFacet(string propName, IEnumerable<KeyValuePair<int,int>> intervals)
        => RawIntervalFacet(propName,string.Join(',',intervals.Select(i => $"{i.Key}-{i.Value}")));
    
    public static IFacetBuilder RawIntervalFacet(string propName, int interval)
        => RawIntervalFacet(propName,interval.ToString());
    
    public static IFacetBuilder RawIntervalFacet(string propName, string interval)
        => new RawFacetBuilder($"{propName},interval:{interval}");
}
