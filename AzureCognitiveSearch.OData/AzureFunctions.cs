namespace AzureCognitiveSearch.OData;

public static class AzureFunctions
{
    /// <summary>
    /// functions that adds search in azure cognitive search
    /// </summary>
    /// <param name="str">str to search in</param>
    /// <param name="matches">matching strings</param>
    /// <param name="delimiter">delimiter</param>
    /// <returns>Always true, it is used to build a query</returns>
    public static bool AzureSearchIn(this string str, IEnumerable<string> matches, string delimiter = ",")
    {
        return true;
    }
}
