using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Facets;

public static class FacetBuilder
{
    // countFacet
    public static IFacet RawCountFacet(string propName, uint count)
        => new RawFacet($"{propName},count:{count}");

    // value facets
    public static IFacet RawValuesFacet(string propName, IEnumerable<int> values)
        => RawValueFacet(propName, values.Select(i => i.ToString()));
    
    public static IFacet RawValuesFacet(string propName, IEnumerable<DateTimeOffset> values)
        => RawValueFacet(propName, values.Select(dt => dt.ToString()));

    // base value facet
    private static IFacet RawValueFacet(string propName, IEnumerable<string> values)
        => new RawFacet($"{propName},values:{string.Join('|', values)}");

    // interval facets
    public static IFacet RawValueFacet(string propName, IEnumerable<KeyValuePair<int,int>> intervals)
        => RawValueFacet(propName,string.Join(',',intervals.Select(i => $"{i.Key}-{i.Value}")));
    
    public static IFacet RawValueFacet(string propName, int interval)
        => RawValueFacet(propName,interval.ToString());
    
    public static IFacet RawValueFacet(string propName, string interval)
        => new RawFacet($"{propName},interval:{interval}");
}
