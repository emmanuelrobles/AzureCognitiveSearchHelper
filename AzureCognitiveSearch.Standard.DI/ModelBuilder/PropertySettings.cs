namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

internal class PropertySettings : IPropertySettings
{
    public PropertySettings(string indexName)
    {
        IndexName = indexName;
    }
    
    public PropertySettings()
    {
        IndexName = string.Empty;
    }

    public string IndexName { get; set; } = string.Empty;
}
