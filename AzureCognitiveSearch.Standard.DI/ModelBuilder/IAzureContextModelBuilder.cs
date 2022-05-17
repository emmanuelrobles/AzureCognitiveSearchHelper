using AzureCognitiveSearch.Applications.Contexts;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

/// <summary>
/// Azure context with a model builder support
/// </summary>
public interface IAzureContextModelBuilder : IAzureContext
{
    /// <summary>
    /// Function that builds the model to apply to the context
    /// </summary>
    /// <param name="modelBuilder"></param>
    IEnumerable<IProperty> BuildModel(IModelBuilder modelBuilder);
}
