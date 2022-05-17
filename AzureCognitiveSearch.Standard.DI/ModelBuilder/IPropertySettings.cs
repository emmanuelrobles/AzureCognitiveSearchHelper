namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

/// <summary>
/// Settings that an index can have
/// </summary>
public interface IPropertySettings
{
    /// <summary>
    /// Name of the index that it should point to
    /// </summary>
    public string IndexName { get; set; }
}
