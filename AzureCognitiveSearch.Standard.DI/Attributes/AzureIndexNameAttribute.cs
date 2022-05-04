namespace AzureCognitiveSearch.DI.FromNugetPackage.Attributes;

[AttributeUsage(System.AttributeTargets.Property)] 
public class AzureIndexNameAttribute : Attribute
{
    public string IndexName { get; }

    public AzureIndexNameAttribute(string indexName)
    {
        IndexName = indexName;
    }
}
