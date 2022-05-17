using System.Reflection;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

/// <summary>
/// Model Builder for a given model
/// </summary>
public interface IModelBuilder
{
    /// <summary>
    /// list with all properties and its settings
    /// </summary>
    public IEnumerable<IProperty> Properties { get; set; }
}
